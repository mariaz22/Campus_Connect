import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { X, Calendar, Clock, AlertTriangle, Info } from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../ui/Card';
import { Button } from '../ui/Button';
import { Input } from '../ui/Input';
import { Textarea } from '../ui/Textarea';
import { Label } from '../ui/Label';
import { roomBookingApi } from '../../services/roomBookingApi';
import { campusMapApi } from '../../services/campusMapApi';
import type { CreateRoomBookingRequest } from '../../services/roomBookingApi';
import type { Schedule } from '../../services/campusMapApi';

interface Props {
  roomId: number;
  roomName: string;
  buildingName: string;
  onClose: () => void;
  onSuccess?: () => void;
}

const BOOKING_START_HOUR = 8;  // 8 AM
const BOOKING_END_HOUR = 20;   // 8 PM

export const RequestBookingDialog = ({ roomId, roomName, buildingName, onClose, onSuccess }: Props) => {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [conflictWarning, setConflictWarning] = useState<string | null>(null);
  const [timeError, setTimeError] = useState<string | null>(null);
  const [existingSchedules, setExistingSchedules] = useState<Schedule[]>([]);
  const [formData, setFormData] = useState<CreateRoomBookingRequest>({
    title: '',
    description: '',
    roomId: roomId,
    startTime: '',
    endTime: '',
    recurrencePattern: '',
    recurrenceEndDate: ''
  });

  // Load schedules when start time changes
  useEffect(() => {
    if (formData.startTime) {
      const startDate = new Date(formData.startTime);
      const dateString = `${startDate.getFullYear()}-${String(startDate.getMonth() + 1).padStart(2, '0')}-${String(startDate.getDate()).padStart(2, '0')}`;

      campusMapApi.getRoomSchedules(roomId, dateString)
        .then(schedules => {
          setExistingSchedules(schedules);
          checkForConflicts(formData.startTime, formData.endTime, schedules);
        })
        .catch(err => console.error('Failed to load schedules:', err));
    }
  }, [formData.startTime, roomId]);

  // Check for conflicts and time validation when times change
  useEffect(() => {
    if (formData.startTime && formData.endTime) {
      validateTime(formData.startTime, formData.endTime);
      checkForConflicts(formData.startTime, formData.endTime, existingSchedules);
    } else {
      setTimeError(null);
      setConflictWarning(null);
    }
  }, [formData.startTime, formData.endTime, existingSchedules]);

  const validateTime = (startTimeStr: string, endTimeStr: string) => {
    const startTime = new Date(startTimeStr);
    const endTime = new Date(endTimeStr);

    // Check if end time is after start time
    if (endTime <= startTime) {
      setTimeError('Ora de terminare trebuie să fie după ora de începere.');
      return false;
    }

    // Check if booking spans multiple days
    if (startTime.toDateString() !== endTime.toDateString()) {
      setTimeError('Rezervările nu pot să se întindă pe mai multe zile.');
      return false;
    }

    // Check if booking is within allowed hours (8 AM - 8 PM)
    if (startTime.getHours() < BOOKING_START_HOUR || startTime.getHours() >= BOOKING_END_HOUR) {
      setTimeError(`Rezervările sunt permise doar între ${BOOKING_START_HOUR}:00 și ${BOOKING_END_HOUR}:00. Ora de începere este în afara intervalului permis.`);
      return false;
    }

    if (endTime.getHours() < BOOKING_START_HOUR || endTime.getHours() > BOOKING_END_HOUR ||
        (endTime.getHours() === BOOKING_END_HOUR && endTime.getMinutes() > 0)) {
      setTimeError(`Rezervările sunt permise doar între ${BOOKING_START_HOUR}:00 și ${BOOKING_END_HOUR}:00. Ora de terminare este în afara intervalului permis.`);
      return false;
    }

    setTimeError(null);
    return true;
  };

  const checkForConflicts = (startTimeStr: string, endTimeStr: string, schedules: Schedule[]) => {
    if (!startTimeStr || !endTimeStr) {
      setConflictWarning(null);
      return;
    }

    const requestStart = new Date(startTimeStr);
    const requestEnd = new Date(endTimeStr);

    const conflicts = schedules.filter(schedule => {
      const scheduleStart = new Date(schedule.startTime);
      const scheduleEnd = new Date(schedule.endTime);

      // Check if there's any overlap
      return (requestStart < scheduleEnd && requestEnd > scheduleStart);
    });

    if (conflicts.length > 0) {
      const conflictTimes = conflicts.map(c =>
        `${new Date(c.startTime).toLocaleTimeString('ro-RO', { hour: '2-digit', minute: '2-digit' })} - ${new Date(c.endTime).toLocaleTimeString('ro-RO', { hour: '2-digit', minute: '2-digit' })} (${c.title})`
      ).join(', ');
      setConflictWarning(`Sala este deja rezervată în intervalul: ${conflictTimes}`);
    } else {
      setConflictWarning(null);
    }
  };

  const isFormValid = () => {
    return formData.title.trim() !== '' &&
           formData.startTime !== '' &&
           formData.endTime !== '' &&
           !timeError &&
           !conflictWarning;
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError(null);

    if (!isFormValid()) {
      if (timeError) {
        setError(timeError);
      } else if (conflictWarning) {
        setError('Nu puteți rezerva sala în acest interval deoarece există un conflict.');
      } else {
        setError('Vă rugăm să completați toate câmpurile obligatorii.');
      }
      return;
    }

    setLoading(true);

    try {
      const request: CreateRoomBookingRequest = {
        ...formData,
        recurrencePattern: formData.recurrencePattern || undefined,
        recurrenceEndDate: formData.recurrenceEndDate || undefined
      };

      await roomBookingApi.createBookingRequest(request);
      onSuccess?.();
      onClose();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Nu s-a putut trimite cererea de rezervare');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
      <motion.div
        initial={{ opacity: 0, scale: 0.95 }}
        animate={{ opacity: 1, scale: 1 }}
        exit={{ opacity: 0, scale: 0.95 }}
        className="w-full max-w-2xl max-h-[90vh] overflow-y-auto"
      >
        <Card>
          <CardHeader>
            <div className="flex items-start justify-between">
              <div>
                <CardTitle className="text-2xl">Cerere Rezervare Sală</CardTitle>
                <p className="text-sm text-muted-foreground mt-1">
                  {roomName} - {buildingName}
                </p>
              </div>
              <Button variant="ghost" size="icon" onClick={onClose}>
                <X className="h-5 w-5" />
              </Button>
            </div>
          </CardHeader>

          <CardContent>
            <form onSubmit={handleSubmit} className="space-y-4">
              {/* Info about booking hours */}
              <div className="p-3 bg-blue-50 border border-blue-200 text-blue-800 rounded-md text-sm flex items-start gap-2">
                <Info className="h-5 w-5 flex-shrink-0 mt-0.5" />
                <span>
                  Rezervările sunt permise doar între <strong>{BOOKING_START_HOUR}:00</strong> și <strong>{BOOKING_END_HOUR}:00</strong>.
                </span>
              </div>

              {error && (
                <div className="p-3 bg-red-100 border border-red-300 text-red-800 rounded-md text-sm">
                  {error}
                </div>
              )}

              {timeError && (
                <div className="p-3 bg-red-100 border border-red-300 text-red-800 rounded-md text-sm flex items-start gap-2">
                  <AlertTriangle className="h-5 w-5 flex-shrink-0 mt-0.5" />
                  <span>{timeError}</span>
                </div>
              )}

              {conflictWarning && !timeError && (
                <div className="p-3 bg-red-100 border border-red-300 text-red-800 rounded-md text-sm flex items-start gap-2">
                  <AlertTriangle className="h-5 w-5 flex-shrink-0 mt-0.5" />
                  <span>{conflictWarning}</span>
                </div>
              )}

              <div>
                <Label htmlFor="title">Titlu *</Label>
                <Input
                  id="title"
                  value={formData.title}
                  onChange={(e) => setFormData({ ...formData, title: e.target.value })}
                  placeholder="Ex: Prezentare proiect IS"
                  required
                />
              </div>

              <div>
                <Label htmlFor="description">Descriere</Label>
                <Textarea
                  id="description"
                  value={formData.description}
                  onChange={(e) => setFormData({ ...formData, description: e.target.value })}
                  placeholder="Detalii despre rezervare..."
                  rows={3}
                />
              </div>

              <div className="grid grid-cols-2 gap-4">
                <div>
                  <Label htmlFor="startTime">
                    <Clock className="inline h-4 w-4 mr-1" />
                    Data și ora începerii *
                  </Label>
                  <Input
                    id="startTime"
                    type="datetime-local"
                    value={formData.startTime}
                    onChange={(e) => setFormData({ ...formData, startTime: e.target.value })}
                    required
                  />
                  <p className="text-xs text-muted-foreground mt-1">
                    Între {BOOKING_START_HOUR}:00 - {BOOKING_END_HOUR}:00
                  </p>
                </div>

                <div>
                  <Label htmlFor="endTime">
                    <Clock className="inline h-4 w-4 mr-1" />
                    Data și ora terminării *
                  </Label>
                  <Input
                    id="endTime"
                    type="datetime-local"
                    value={formData.endTime}
                    onChange={(e) => setFormData({ ...formData, endTime: e.target.value })}
                    required
                  />
                  <p className="text-xs text-muted-foreground mt-1">
                    Între {BOOKING_START_HOUR}:00 - {BOOKING_END_HOUR}:00
                  </p>
                </div>
              </div>

              <div>
                <Label htmlFor="recurrencePattern">
                  <Calendar className="inline h-4 w-4 mr-1" />
                  Pattern recurrent (opțional)
                </Label>
                <select
                  id="recurrencePattern"
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500"
                  value={formData.recurrencePattern}
                  onChange={(e) => setFormData({ ...formData, recurrencePattern: e.target.value })}
                >
                  <option value="">Fără recurrență</option>
                  <option value="Daily">Zilnic</option>
                  <option value="Weekly">Săptămânal</option>
                  <option value="BiWeekly">La 2 săptămâni</option>
                  <option value="Monthly">Lunar</option>
                </select>
              </div>

              {formData.recurrencePattern && (
                <div>
                  <Label htmlFor="recurrenceEndDate">Data terminării recurenței</Label>
                  <Input
                    id="recurrenceEndDate"
                    type="date"
                    value={formData.recurrenceEndDate}
                    onChange={(e) => setFormData({ ...formData, recurrenceEndDate: e.target.value })}
                  />
                </div>
              )}

              <div className="bg-blue-50 border border-blue-200 rounded-md p-4">
                <p className="text-sm text-blue-800">
                  <strong>Notă:</strong> Cererea va fi revizuită de un administrator.
                  Veți primi o notificare când cererea va fi aprobată sau respinsă.
                </p>
              </div>

              <div className="flex gap-2 justify-end pt-4">
                <Button type="button" variant="outline" onClick={onClose} disabled={loading}>
                  Anulează
                </Button>
                <Button type="submit" disabled={loading || !isFormValid()}>
                  {loading ? 'Se trimite...' : 'Trimite Cerere'}
                </Button>
              </div>
            </form>
          </CardContent>
        </Card>
      </motion.div>
    </div>
  );
};
