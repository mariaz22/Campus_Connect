import { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { Clock, MapPin, User, CheckCircle, XCircle, Calendar } from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Badge } from '../../components/ui/Badge';
import { Textarea } from '../../components/ui/Textarea';
import { roomBookingApi } from '../../services/roomBookingApi';
import type { RoomBookingRequest } from '../../services/roomBookingApi';

export const BookingRequests = () => {
  const [requests, setRequests] = useState<RoomBookingRequest[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [selectedRequest, setSelectedRequest] = useState<RoomBookingRequest | null>(null);
  const [rejectionReason, setRejectionReason] = useState('');
  const [processing, setProcessing] = useState(false);

  useEffect(() => {
    loadPendingRequests();
  }, []);

  const loadPendingRequests = async () => {
    try {
      setLoading(true);
      const data = await roomBookingApi.getPendingRequests();
      setRequests(data);
      setError(null);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load requests');
    } finally {
      setLoading(false);
    }
  };

  const handleApprove = async (requestId: number) => {
    try {
      setProcessing(true);
      await roomBookingApi.approveRequest(requestId);
      await loadPendingRequests();
      setSelectedRequest(null);
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to approve request');
    } finally {
      setProcessing(false);
    }
  };

  const handleReject = async (requestId: number) => {
    try {
      setProcessing(true);
      await roomBookingApi.rejectRequest(requestId, rejectionReason);
      await loadPendingRequests();
      setSelectedRequest(null);
      setRejectionReason('');
    } catch (err) {
      alert(err instanceof Error ? err.message : 'Failed to reject request');
    } finally {
      setProcessing(false);
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
          <h1 className="text-3xl font-bold">Room Booking Requests</h1>
          <p className="text-muted-foreground mt-2">
            Manage booking requests received from users
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
                <p>No pending requests</p>
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
                          <User className="h-4 w-4" />
                          <span>{request.requestedByUserName}</span>
                          <span className="text-xs">({request.requestedByUserEmail})</span>
                        </div>

                        <div className="flex items-center gap-2 text-muted-foreground">
                          <Clock className="h-4 w-4" />
                          <span>
                            {new Date(request.startTime).toLocaleString('ro-RO', {
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
                            {new Date(request.endTime).toLocaleString('ro-RO', {
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
                            <> until {new Date(request.recurrenceEndDate).toLocaleDateString('ro-RO')}</>
                          )}
                        </div>
                      )}

                      <div className="text-xs text-muted-foreground">
                        Request sent at: {new Date(request.createdAt).toLocaleString('ro-RO')}
                      </div>
                    </div>

                    {request.status === 'Pending' && (
                      <div className="flex gap-2">
                        <Button
                          onClick={() => handleApprove(request.id)}
                          disabled={processing}
                          className="gap-2 bg-green-600 hover:bg-green-700"
                          size="sm"
                        >
                          <CheckCircle className="h-4 w-4" />
                          Approve
                        </Button>
                        <Button
                          onClick={() => setSelectedRequest(request)}
                          disabled={processing}
                          variant="destructive"
                          className="gap-2"
                          size="sm"
                        >
                          <XCircle className="h-4 w-4" />
                          Reject
                        </Button>
                      </div>
                    )}
                  </div>
                </CardContent>
              </Card>
            ))}
          </div>
        )}

        {/* Rejection Dialog */}
        {selectedRequest && (
          <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4">
            <motion.div
              initial={{ opacity: 0, scale: 0.95 }}
              animate={{ opacity: 1, scale: 1 }}
              className="w-full max-w-md"
            >
              <Card>
                <CardHeader>
                  <CardTitle>Reject Request</CardTitle>
                </CardHeader>
                <CardContent className="space-y-4">
                  <div>
                    <p className="text-sm text-muted-foreground mb-2">
                      Request from: <strong>{selectedRequest.requestedByUserName}</strong>
                    </p>
                    <p className="text-sm text-muted-foreground">
                      Room: <strong>{selectedRequest.roomName}</strong>
                    </p>
                  </div>

                  <div>
                    <label className="text-sm font-medium mb-2 block">
                      Reason for rejection (optional)
                    </label>
                    <Textarea
                      value={rejectionReason}
                      onChange={(e) => setRejectionReason(e.target.value)}
                      rows={4}
                    />
                  </div>

                  <div className="flex gap-2 justify-end">
                    <Button
                      variant="outline"
                      onClick={() => {
                        setSelectedRequest(null);
                        setRejectionReason('');
                      }}
                      disabled={processing}
                    >
                      Cancel
                    </Button>
                    <Button
                      variant="destructive"
                      onClick={() => handleReject(selectedRequest.id)}
                      disabled={processing}
                    >
                      {processing ? 'Rejecting...' : 'Confirm Rejection'}
                    </Button>
                  </div>
                </CardContent>
              </Card>
            </motion.div>
          </div>
        )}
      </motion.div>
    </div>
  );
};

export default BookingRequests;