import { useState } from 'react';
import { motion } from 'framer-motion';
import { DoorOpen, Users, MapPin, Clock } from 'lucide-react';
import { Badge } from '../ui/Badge';
import { RoomDetailsModal } from './RoomDetailsModal';
import type { Room } from '../../services/campusMapApi';

interface Props {
  room: Room;
}

export const RoomCard = ({ room }: Props) => {
  const [showDetails, setShowDetails] = useState(false);

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Free': return 'bg-green-500 text-white';
      case 'Occupied': return 'bg-red-500 text-white';
      case 'OccupiedSoon': return 'bg-yellow-500 text-white';
      default: return 'bg-gray-500 text-white';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Free': return 'Free';
      case 'Occupied': return 'Occupied';
      case 'OccupiedSoon': return 'Occupied Soon';
      default: return 'Unknown';
    }
  };

  return (
    <>
      <motion.div
        whileHover={{ scale: 1.02 }}
        whileTap={{ scale: 0.98 }}
        className="p-4 border-2 border-border rounded-lg hover:border-primary/50 transition-all cursor-pointer"
        onClick={() => setShowDetails(true)}
      >
        <div className="flex items-center justify-between">
          <div className="flex items-center gap-3 flex-1">
            <DoorOpen className="h-5 w-5 text-primary" />
            <div className="flex-1 min-w-0">
              <h4 className="font-semibold text-sm truncate">{room.name}</h4>
              <div className="flex items-center gap-3 mt-1">
                {room.floor && (
                  <span className="text-xs text-muted-foreground flex items-center gap-1">
                    <MapPin className="h-3 w-3" />
                    {room.floor}
                  </span>
                )}
                {room.capacity && (
                  <span className="text-xs text-muted-foreground flex items-center gap-1">
                    <Users className="h-3 w-3" />
                    {room.capacity}
                  </span>
                )}
              </div>
            </div>
          </div>

          <Badge className={`${getStatusColor(room.currentStatus)} border-0 text-xs`}>
            {getStatusText(room.currentStatus)}
          </Badge>
        </div>

        {room.occupiedUntil && (
          <div className="mt-2 text-xs text-muted-foreground flex items-center gap-1">
            <Clock className="h-3 w-3" />
            Occupied until {new Date(room.occupiedUntil).toLocaleTimeString('en-US', {
              hour: '2-digit',
              minute: '2-digit'
            })}
          </div>
        )}
      </motion.div>

      {showDetails && (
        <RoomDetailsModal
          roomId={room.id}
          onClose={() => setShowDetails(false)}
        />
      )}
    </>
  );
};
