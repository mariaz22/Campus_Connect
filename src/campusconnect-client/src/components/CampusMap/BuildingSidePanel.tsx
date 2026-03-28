import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import { X, MapPin, DoorOpen } from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../ui/Card';
import { Button } from '../ui/Button';
import { RoomCard } from './RoomCard';
import { campusMapApi } from '../../services/campusMapApi';
import type { Building, Room } from '../../services/campusMapApi';

interface Props {
  building: Building;
  onClose: () => void;
}

export const BuildingSidePanel = ({ building, onClose }: Props) => {
  const [rooms, setRooms] = useState<Room[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadRooms();
  }, [building.id]);

  const loadRooms = async () => {
    try {
      const data = await campusMapApi.getRoomsByBuilding(building.id);
      setRooms(data);
    } catch (error) {
      console.error('Error loading rooms:', error);
    } finally {
      setLoading(false);
    }
  };

  const freeRooms = rooms.filter(r => r.currentStatus === 'Free').length;
  const occupiedRooms = rooms.filter(r => r.currentStatus === 'Occupied').length;

  return (
    <AnimatePresence>
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        exit={{ opacity: 0, y: 20 }}
      >
        <Card>
          <CardHeader>
            <div className="flex items-start justify-between">
              <div>
                <CardTitle className="text-2xl">{building.name}</CardTitle>
                <div className="flex items-center gap-2 mt-2 text-sm text-muted-foreground">
                  <MapPin className="h-4 w-4" />
                  <span>{building.address}</span>
                </div>
                {building.description && (
                  <p className="text-sm text-muted-foreground mt-2">{building.description}</p>
                )}
              </div>
              <Button variant="ghost" size="icon" onClick={onClose}>
                <X className="h-5 w-5" />
              </Button>
            </div>
          </CardHeader>

          <CardContent>
            {/* Stats */}
            <div className="grid grid-cols-3 gap-4 mb-6">
              <div className="text-center p-4 bg-muted rounded-lg">
                <DoorOpen className="h-6 w-6 mx-auto mb-2 text-primary" />
                <div className="text-2xl font-bold">{rooms.length}</div>
                <div className="text-xs text-muted-foreground">Total rooms</div>
              </div>
              <div className="text-center p-4 bg-green-50 dark:bg-green-950/20 rounded-lg">
                <div className="text-2xl font-bold text-green-600">{freeRooms}</div>
                <div className="text-xs text-muted-foreground">Free Now</div>
              </div>
              <div className="text-center p-4 bg-red-50 dark:bg-red-950/20 rounded-lg">
                <div className="text-2xl font-bold text-red-600">{occupiedRooms}</div>
                <div className="text-xs text-muted-foreground">Occupied</div>
              </div>
            </div>

            {/* Room List */}
            <div className="space-y-3">
              <h3 className="font-semibold text-lg mb-3">Rooms</h3>
              {loading ? (
                <div className="text-center py-8">Loading rooms...</div>
              ) : (
                <div className="space-y-2 max-h-96 overflow-y-auto">
                  {rooms.map((room) => (
                    <RoomCard key={room.id} room={room} />
                  ))}
                </div>
              )}
            </div>
          </CardContent>
        </Card>
      </motion.div>
    </AnimatePresence>
  );
};
