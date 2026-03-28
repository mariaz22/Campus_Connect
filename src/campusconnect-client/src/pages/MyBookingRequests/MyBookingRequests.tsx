import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Clock, MapPin, Calendar, Trash2 } from 'lucide-react';
import { Card, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { roomBookingApi } from '../../services/roomBookingApi';
import type { RoomBookingRequest } from '../../services/roomBookingApi';

export const MyBookingRequests = () => {
  const [requests, setRequests] = useState<RoomBookingRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    loadMyRequests();
  }, []);

  const loadMyRequests = async () => {
    try {
      setLoading(true);
      const data = await roomBookingApi.getMyRequests();
      setRequests(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load requests');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (requestId: number) => {
    if (!confirm('Are you sure you want to delete this request?')) {
      return;
    }

    try {
      await roomBookingApi.deleteRequest(requestId);
      await loadMyRequests();
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to delete request');
    }
  };

  const getStatusColor = (status: string) => {
    switch (status) {
      case 'Pending': return 'bg-yellow-500 text-white';
      case 'Approved': return 'bg-green-500 text-white';
      case 'Rejected': return 'bg-red-500 text-white';
      default: return 'bg-gray-500 text-white';
    }
  };

  const getStatusText = (status: string) => {
    switch (status) {
      case 'Pending': return 'Pending';
      case 'Approved': return 'Approved';
      case 'Rejected': return 'Rejected';
      default: return status;
    }
  };

  if (loading) {
    return (
      <div className="container mx-auto px-4 py-8">
        <div className="text-center">Loading requests...</div>
      </div>
    );
  }

  return (
    <div className="container mx-auto px-4 py-8">
      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="space-y-6"
      >
        <div>
          <h1 className="text-3xl font-bold">My Booking Requests</h1>
          <p className="text-muted-foreground mt-2">
            View the status of your room booking requests
          </p>
        </div>

        {error && (
          <div className="p-4 bg-red-100 border border-red-300 text-red-800 rounded-md">
            {error}
          </div>
        )}

        {requests.length === 0 ? (
          <Card>
            <CardContent className="py-12">
              <div className="text-center text-muted-foreground">
                <Calendar className="h-12 w-12 mx-auto mb-4 opacity-50" />
                <p>You have no booking requests</p>
              </div>
            </CardContent>
          </Card>
        ) : (
          <div className="grid gap-4">
            {requests.map((request) => (
              <Card key={request.id} className="hover:shadow-lg transition-shadow">
                <CardContent className="p-6">
                  <div className="flex items-start justify-between gap-4">
                    <div className="flex-1 space-y-3">
                      <div className="flex items-start justify-between">
                        <div>
                          <h3 className="text-xl font-semibold">{request.title}</h3>
                          {request.description && (
                            <p className="text-sm text-muted-foreground mt-1">
                              {request.description}
                            </p>
                          )}
                        </div>
                        <Badge className={getStatusColor(request.status)}>
                          {getStatusText(request.status)}
                        </Badge>
                      </div>

                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
                        <div className="flex items-center gap-2 text-muted-foreground">
                          <MapPin className="h-4 w-4" />
                          <span>{request.roomName} - {request.buildingName}</span>
                        </div>

                        <div className="flex items-center gap-2 text-muted-foreground">
                          <Clock className="h-4 w-4" />
                          <span>
                            {new Date(request.startTime).toLocaleString('en-US', {
                              day: '2-digit',
                              month: 'short',
                              year: 'numeric',
                              hour: '2-digit',
                              minute: '2-digit'
                            })}
                          </span>
                        </div>

                        <div className="flex items-center gap-2 text-muted-foreground">
                          <Clock className="h-4 w-4" />
                          <span>
                            {new Date(request.endTime).toLocaleString('en-US', {
                              day: '2-digit',
                              month: 'short',
                              year: 'numeric',
                              hour: '2-digit',
                              minute: '2-digit'
                            })}
                          </span>
                        </div>
                      </div>

                      {request.recurrencePattern && (
                        <div className="text-sm text-muted-foreground">
                          <strong>Recurrence:</strong> {request.recurrencePattern}
                          {request.recurrenceEndDate && (
                            <> until {new Date(request.recurrenceEndDate).toLocaleDateString('en-US')}</>
                          )}
                        </div>
                      )}

                      <div className="text-xs text-muted-foreground">
                        Request sent at: {new Date(request.createdAt).toLocaleString('en-US')}
                      </div>

                      {request.status === 'Approved' && request.reviewedByAdminName && (
                        <div className="text-sm text-green-700 bg-green-50 p-3 rounded-md">
                          ✓ Approved by {request.reviewedByAdminName} at{' '}
                          {new Date(request.reviewedAt!).toLocaleString('en-US')}
                        </div>
                      )}

                      {request.status === 'Rejected' && (
                        <div className="text-sm text-red-700 bg-red-50 p-3 rounded-md">
                          ✗ Rejected by {request.reviewedByAdminName} at{' '}
                          {new Date(request.reviewedAt!).toLocaleString('en-US')}
                          {request.rejectionReason && (
                            <div className="mt-2">
                              <strong>Reason:</strong> {request.rejectionReason}
                            </div>
                          )}
                        </div>
                      )}
                    </div>

                    {request.status === 'Pending' && (
                      <Button
                        onClick={() => handleDelete(request.id)}
                        variant="destructive"
                        size="sm"
                        className="gap-2"
                      >
                        <Trash2 className="h-4 w-4" />
                        Delete
                      </Button>
                    )}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}
      </motion.div>
    </div>
  );
};
export default MyBookingRequests;
