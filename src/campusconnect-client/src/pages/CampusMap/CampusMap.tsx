import { useEffect, useState } from 'react';
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet';
import { motion } from 'framer-motion';
import { MapPin, Building as BuildingIcon, Search, Plus } from 'lucide-react';
import L from 'leaflet';
import { Layout } from '../../components/Layout';
import { Card, CardContent } from '../../components/ui/Card';
import { Input } from '../../components/ui/Input';
import { Button } from '../../components/ui/Button';
import { BuildingSidePanel } from '../../components/CampusMap/BuildingSidePanel';
import { CreateScheduleDialog } from '../../components/CampusMap/CreateScheduleDialog';
import { campusMapApi } from '../../services/campusMapApi';
import type { Building } from '../../services/campusMapApi';

// Fix Leaflet default marker icon issue with Vite
delete (L.Icon.Default.prototype as any)._getIconUrl;
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
});

//  Icon normal + icon selectat (mov)
const normalIcon = new L.Icon({
  iconUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
  iconRetinaUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon-2x.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
  iconSize: [25, 41],
  iconAnchor: [12, 41],
});

const selectedIcon = new L.Icon({
  iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-violet.png',
  shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
  iconSize: [30, 45],
  iconAnchor: [15, 45],
});

//  centreaza harta pe cladirea selectata
const MapCenterOnBuilding = ({ building }: { building: Building | null }) => {
  const map = useMap();

  useEffect(() => {
    if (!building) return;
    map.setView([building.latitude, building.longitude], 17, { animate: true });
  }, [building, map]);

  return null;
};

const CampusMap = () => {
  const [buildings, setBuildings] = useState<Building[]>([]);
  const [selectedBuilding, setSelectedBuilding] = useState<Building | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [createDialogOpen, setCreateDialogOpen] = useState(false);

  // Get user info for admin/professor check
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isAdminOrProfessor = user.role === 'Admin' || user.role === 'Professor';

  // Center map on Piața Universității
  const center: [number, number] = [44.4361, 26.0986];

  useEffect(() => {
    loadBuildings();
  }, []);

  const loadBuildings = async () => {
    try {
      const data = await campusMapApi.getAllBuildings();
      setBuildings(data);
    } catch (error) {
      console.error('Error loading buildings:', error);
    }
  };

  const filteredBuildings = buildings.filter((b) =>
    b.name.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-blue-500 via-cyan-500 to-teal-500 p-8 text-white shadow-2xl"
        >
          <div className="absolute inset-0 bg-[url('data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iNjAiIGhlaWdodD0iNjAiIHZpZXdCb3g9IjAgMCA2MCA2MCIgeG1sbnM9Imh0dHA6Ly93d3cudzMub3JnLzIwMDAvc3ZnIj48ZyBmaWxsPSJub25lIiBmaWxsLXJ1bGU9ImV2ZW5vZGQiPjxnIGZpbGw9IiNmZmYiIGZpbGwtb3BhY2l0eT0iMC4xIj48cGF0aCBkPSJNMzYgMzRjMC0yLjIxIDEuNzktNCA0LTRzNCAxLjc5IDQgNC0xLjc5IDQtNCA0LTQtMS43OS00LTR6bTAtMTZjMC0yLjIxIDEuNzktNCA0LTRzNCAxLjc5IDQgNC0xLjc5IDQtNCA0LTQtMS43OS00LTR6Ii8+PC9nPjwvZz48L3N2Zz4=')] opacity-30"></div>
          <div className="relative z-10">
            <div className="flex items-center justify-between mb-2">
              <div className="flex items-center gap-3">
                <div className="p-3 bg-white/20 backdrop-blur-sm rounded-xl">
                  <MapPin className="h-8 w-8" />
                </div>
                <div>
                  <h1 className="text-4xl font-bold">Campus Map UniBuc</h1>
                  <p className="text-white/80 mt-1">Interactive navigation in campus</p>
                </div>
              </div>
              <Button
                onClick={() => setCreateDialogOpen(true)}
                className="bg-white text-blue-600 hover:bg-white/90"
              >
                <Plus className="h-4 w-4 mr-2" />
                {isAdminOrProfessor ? 'Add Schedule' : 'Request Booking'}
              </Button>
            </div>
          </div>
        </motion.div>

        {/* Map Container + Side Panel */}
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
          {/* Building List Panel */}
          <div className="lg:col-span-1">
            <Card>
              <CardContent className="p-4">
                <div className="mb-4">
                  <div className="relative">
                    <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 h-4 w-4 text-muted-foreground" />
                    <Input
                      type="text"
                      placeholder="Search buildings..."
                      value={searchTerm}
                      onChange={(e) => setSearchTerm(e.target.value)}
                      className="pl-10"
                    />
                  </div>
                </div>

                <div className="space-y-2 max-h-[600px] overflow-y-auto">
                  {filteredBuildings.map((building) => (
                    <motion.div
                      key={building.id}
                      whileHover={{ scale: 1.02 }}
                      whileTap={{ scale: 0.98 }}
                      onClick={() => setSelectedBuilding(building)}
                      className={`p-4 rounded-lg border-2 cursor-pointer transition-all ${
                        selectedBuilding?.id === building.id
                          ? 'border-primary bg-primary/10'
                          : 'border-border hover:border-primary/50'
                      }`}
                    >
                      <div className="flex items-start gap-3">
                        <BuildingIcon className="h-5 w-5 text-primary flex-shrink-0 mt-1" />
                        <div className="flex-1 min-w-0">
                          <h3 className="font-semibold text-sm truncate">{building.name}</h3>
                          <p className="text-xs text-muted-foreground truncate">{building.address}</p>
                          <p className="text-xs text-muted-foreground mt-1">{building.roomsCount} rooms</p>
                        </div>
                      </div>
                    </motion.div>
                  ))}
                </div>
              </CardContent>
            </Card>
          </div>

          {/* Map */}
          <div className="lg:col-span-2">
            <Card className="overflow-hidden">
              <div className="h-[600px]">
                <MapContainer
                  center={center}
                  zoom={16}
                  className="h-full w-full rounded-lg"
                  style={{ zIndex: 0 }}
                >
                  <TileLayer
                    attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>'
                    url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                  />

                  {/* ✅ centrează pe clădire selectată */}
                  <MapCenterOnBuilding building={selectedBuilding} />

                  {filteredBuildings.map((building) => (
                    <Marker
                      key={building.id}
                      position={[building.latitude, building.longitude]}
                      // ✅ icon mov dacă e selectată
                      icon={selectedBuilding?.id === building.id ? selectedIcon : normalIcon}
                      eventHandlers={{
                        click: () => setSelectedBuilding(building),
                      }}
                    >
                      <Popup>
                        <div className="text-sm">
                          <h3 className="font-bold">{building.name}</h3>
                          <p className="text-xs text-muted-foreground">{building.address}</p>
                        </div>
                      </Popup>
                    </Marker>
                  ))}
                </MapContainer>
              </div>
            </Card>
          </div>
        </div>

        {/* Building Details Panel */}
        {selectedBuilding && (
          <BuildingSidePanel building={selectedBuilding} onClose={() => setSelectedBuilding(null)} />
        )}

        {/* Create Schedule Dialog */}
        <CreateScheduleDialog
          open={createDialogOpen}
          onOpenChange={setCreateDialogOpen}
          onScheduleCreated={loadBuildings}
        />
      </div>
    </Layout>
  );
};

export default CampusMap;
