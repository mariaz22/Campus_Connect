import { useState, useEffect } from 'react';
import { Calendar, Clock, AlertCircle, Info } from 'lucide-react';
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from '../ui/Dialog';
import { Label } from '../ui/Label';
import { Input } from '../ui/Input';
import { Textarea } from '../ui/Textarea';
import { Select } from '../ui/Select';
import { Button } from '../ui/Button';
import { campusMapApi } from '../../services/campusMapApi';
import type { Building, Room, CreateScheduleRequest, CreateReservationRequest } from '../../services/campusMapApi';

interface CreateScheduleDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  onScheduleCreated?: () => void;
  preselectedBuildingId?: number;
  preselectedRoomId?: number;
}

export function CreateScheduleDialog({
  open,
  onOpenChange,
  onScheduleCreated,
  preselectedBuildingId,
  preselectedRoomId,
}: CreateScheduleDialogProps) {
  const [buildings, setBuildings] = useState<Building[]>([]);
  const [rooms, setRooms] = useState<Room[]>([]);
  const [selectedBuildingId, setSelectedBuildingId] = useState<number | undefined>(preselectedBuildingId);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);

  // Get user role
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const isAdminOrProfessor = user.role === 'Admin' || user.role === 'Professor';

  const [formData, setFormData] = useState<Omit<CreateScheduleRequest, 'roomId'> & { roomId: number | '' }>({
    title: '',
    description: '',
    roomId: preselectedRoomId || '',
    startTime: '',
    endTime: '',
    recurrencePattern: '',
    recurrenceEndDate: '',
  });

  const [scheduleDate, setScheduleDate] = useState('');
  const [startTimeOnly, setStartTimeOnly] = useState('');
  const [endTimeOnly, setEndTimeOnly] = useState('');

  useEffect(() => {
    if (open) {
      loadBuildings();
      if (preselectedBuildingId) {
        loadRooms(preselectedBuildingId);
      }
    }
  }, [open, preselectedBuildingId]);

  useEffect(() => {
    if (selectedBuildingId) {
      loadRooms(selectedBuildingId);
    } else {
      setRooms([]);
    }
  }, [selectedBuildingId]);

  const loadBuildings = async () => {
    try {
      const data = await campusMapApi.getAllBuildings();
      setBuildings(data);
    } catch (error) {
      console.error('Error loading buildings:', error);
      setError('The buildings could not be loaded');
    }
  };

  const loadRooms = async (buildingId: number) => {
    try {
      const data = await campusMapApi.getRoomsByBuilding(buildingId);
      setRooms(data);
    } catch (error) {
      console.error('Error loading rooms:', error);
      setError('The rooms could not be loaded');
    }
  };

  // Validate time is between 8 AM and 8 PM
  const validateTime = (startTime: string, endTime: string): string | null => {
    if (!startTime || !endTime) return null;

    const [startHour] = startTime.split(':').map(Number);
    const [endHour, endMin] = endTime.split(':').map(Number);

    if (startHour < 8) {
      return 'Start time must be after 08:00';
    }
    if (endHour > 20 || (endHour === 20 && endMin > 0)) {
      return 'End time must be before 20:00';
    }
    if (startTime >= endTime) {
      return 'End time must be after start time';
    }
    return null;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);
    setSuccess(null);

    if (!formData.roomId) {
      setError('Select a room');
      return;
    }

    if (!formData.title || !scheduleDate || !startTimeOnly || !endTimeOnly) {
      setError('Please fill in all required fields');
      return;
    }

    // Validate time constraints
    const timeError = validateTime(startTimeOnly, endTimeOnly);
    if (timeError) {
      setError(timeError);
      return;
    }

    setIsLoading(true);

    try {
      // Combine date with time to create DateTime
      const startDateTime = new Date(`${scheduleDate}T${startTimeOnly}`);
      const endDateTime = new Date(`${scheduleDate}T${endTimeOnly}`);

      if (isAdminOrProfessor) {
        // Admin/Professor: Create schedule directly
        const request: CreateScheduleRequest = {
          title: formData.title,
          description: formData.description || undefined,
          roomId: formData.roomId as number,
          startTime: startDateTime.toISOString(),
          endTime: endDateTime.toISOString(),
          recurrencePattern: formData.recurrencePattern || undefined,
          recurrenceEndDate: formData.recurrenceEndDate ? new Date(formData.recurrenceEndDate).toISOString() : undefined,
        };

        await campusMapApi.createSchedule(request);
        setSuccess('Schedule created successfully!');
      } else {
        // Regular user: Create reservation request
        const request: CreateReservationRequest = {
          title: formData.title,
          description: formData.description || undefined,
          roomId: formData.roomId as number,
          startTime: startDateTime.toISOString(),
          endTime: endDateTime.toISOString(),
        };

        await campusMapApi.createReservation(request);
        setSuccess('The reservation request has been sent! You will be notified when it is processed.');
      }

      // Reset form after short delay to show success message
      setTimeout(() => {
        setFormData({
          title: '',
          description: '',
          roomId: preselectedRoomId || '',
          startTime: '',
          endTime: '',
          recurrencePattern: '',
          recurrenceEndDate: '',
        });
        setScheduleDate('');
        setStartTimeOnly('');
        setEndTimeOnly('');
        setSuccess(null);

        onScheduleCreated?.();
        onOpenChange(false);
      }, 1500);
    } catch (error: any) {
      console.error('Error creating schedule/reservation:', error);
      setError(error.message || 'Error creating schedule/reservation');
    } finally {
      setIsLoading(false);
    }
  };

  const handleClose = () => {
    setError(null);
    setSuccess(null);
    setFormData({
      title: '',
      description: '',
      roomId: preselectedRoomId || '',
      startTime: '',
      endTime: '',
      recurrencePattern: '',
      recurrenceEndDate: '',
    });
    setScheduleDate('');
    setStartTimeOnly('');
    setEndTimeOnly('');
    onOpenChange(false);
  };

  return (
    <Dialog open={open} onOpenChange={onOpenChange}>
      <DialogContent onClose={handleClose} className="max-h-[90vh] overflow-y-auto">
        <form onSubmit={handleSubmit}>
          <DialogHeader>
            <DialogTitle className="flex items-center gap-2">
              <Calendar className="h-5 w-5" />
              {isAdminOrProfessor ? 'Add New Schedule' : 'Request Room Reservation'}
            </DialogTitle>
            <DialogDescription>
              {isAdminOrProfessor
                ? 'Create a schedule for a room in the campus'
                : 'Submit a reservation request that will be processed by an administrator'
              }
            </DialogDescription>
          </DialogHeader>

          <div className="space-y-4 p-6">
            {/* Info box for time restrictions */}
            <div className="rounded-md bg-blue-50 dark:bg-blue-950/20 p-3 text-sm text-blue-700 dark:text-blue-300 flex items-start gap-2">
              <Info className="h-4 w-4 mt-0.5 flex-shrink-0" />
              <span>Reservations can only be made between 08:00 and 20:00.</span>
            </div>

            {/* Info box for regular users */}
            {!isAdminOrProfessor && (
              <div className="rounded-md bg-amber-50 dark:bg-amber-950/20 p-3 text-sm text-amber-700 dark:text-amber-300 flex items-start gap-2">
                <AlertCircle className="h-4 w-4 mt-0.5 flex-shrink-0" />
                <span>Your request will be sent to an administrator for approval. You will be notified when the request is processed.</span>
              </div>
            )}

            {error && (
              <div className="rounded-md bg-destructive/10 p-3 text-sm text-destructive">
                {error}
              </div>
            )}

            {success && (
              <div className="rounded-md bg-green-100 dark:bg-green-900/20 p-3 text-sm text-green-700 dark:text-green-300">
                {success}
              </div>
            )}

            <div className="space-y-2">
              <Label htmlFor="title">Title *</Label>
              <Input
                id="title"
                placeholder="e.g., Project Meeting, Study Session..."
                value={formData.title}
                onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                required
              />
            </div>

            <div className="space-y-2">
              <Label htmlFor="description">Description</Label>
              <Textarea
                id="description"
                placeholder="Additional details about the event..."
                value={formData.description}
                onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                rows={3}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="building">Building *</Label>
                <Select
                  id="building"
                  value={selectedBuildingId || ''}
                  onChange={(e) => {
                    const buildingId = e.target.value ? Number(e.target.value) : undefined;
                    setSelectedBuildingId(buildingId);
                    setFormData({ ...formData, roomId: '' });
                  }}
                  disabled={!!preselectedBuildingId}
                  required
                >
                  <option value="">Select a building</option>
                  {buildings.map((building) => (
                    <option key={building.id} value={building.id}>
                      {building.name}
                    </option>
                  ))}
                </Select>
              </div>

              <div className="space-y-2">
                <Label htmlFor="room">Room *</Label>
                <Select
                  id="room"
                  value={formData.roomId}
                  onChange={(e) => setFormData({ ...formData, roomId: e.target.value ? Number(e.target.value) : '' })}
                  disabled={!selectedBuildingId || !!preselectedRoomId}
                  required
                >
                  <option value="">Select a room</option>
                  {rooms.map((room) => (
                    <option key={room.id} value={room.id}>
                      {room.name} {room.capacity ? `(${room.capacity} seats)` : ''}
                    </option>
                  ))}
                </Select>
              </div>
            </div>

            <div className="space-y-2">
              <Label htmlFor="scheduleDate" className="flex items-center gap-1">
                <Calendar className="h-3 w-3" />
                Date *
              </Label>
              <Input
                id="scheduleDate"
                type="date"
                value={scheduleDate}
                onChange={(e) => setScheduleDate(e.target.value)}
                min={new Date().toISOString().split('T')[0]}
                required
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              <div className="space-y-2">
                <Label htmlFor="startTime" className="flex items-center gap-1">
                  <Clock className="h-3 w-3" />
                  Start Time * (08:00 - 20:00)
                </Label>
                <Input
                  id="startTime"
                  type="time"
                  value={startTimeOnly}
                  onChange={(e) => setStartTimeOnly(e.target.value)}
                  min="08:00"
                  max="19:59"
                  required
                />
              </div>

              <div className="space-y-2">
                <Label htmlFor="endTime" className="flex items-center gap-1">
                  <Clock className="h-3 w-3" />
                  End Time * (08:00 - 20:00)
                </Label>
                <Input
                  id="endTime"
                  type="time"
                  value={endTimeOnly}
                  onChange={(e) => setEndTimeOnly(e.target.value)}
                  min="08:01"
                  max="20:00"
                  required
                />
              </div>
            </div>

            {/* Only show recurrence options for Admin/Professor */}
            {isAdminOrProfessor && (
              <>
                <div className="space-y-2">
                  <Label htmlFor="recurrencePattern">Recurrence</Label>
                  <Select
                    id="recurrencePattern"
                    value={formData.recurrencePattern}
                    onChange={(e) => setFormData({ ...formData, recurrencePattern: e.target.value })}
                  >
                    <option value="">No recurrence</option>
                    <option value="Daily">Daily</option>
                    <option value="Weekly">Weekly</option>
                    <option value="BiWeekly">Every 2 weeks</option>
                    <option value="Monthly">Monthly</option>
                  </Select>
                </div>

                {formData.recurrencePattern && (
                  <div className="space-y-2">
                    <Label htmlFor="recurrenceEndDate">Recurrence End Date</Label>
                    <Input
                      id="recurrenceEndDate"
                      type="date"
                      value={formData.recurrenceEndDate}
                      onChange={(e) => setFormData({ ...formData, recurrenceEndDate: e.target.value })}
                      min={scheduleDate}
                    />
                  </div>
                )}
              </>
            )}
          </div>

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              onClick={handleClose}
              disabled={isLoading}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={isLoading || !!success}>
              {isLoading
                ? 'Processing...'
                : isAdminOrProfessor
                  ? 'Create Schedule'
                  : 'Submit Request'
              }
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
