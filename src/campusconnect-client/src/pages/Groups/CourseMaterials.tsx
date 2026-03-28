import { useState, useEffect } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  FileText,
  Download,
  Upload,
  Trash2,
  File,
  FileVideo,
  FileImage,
  FileArchive,
  Calendar,
  User,
  X,
  Loader2,
  AlertCircle,
} from 'lucide-react';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';

interface CourseMaterial {
  id: number;
  title: string;
  description?: string;
  fileName: string;
  fileUrl: string;
  fileType: string;
  fileSize: number;
  uploadedByProfessorName: string;
  uploadedAt: string;
}

interface CourseMaterialsProps {
  groupId: number;
  isGroupOwner: boolean;
}

const CourseMaterials = ({ groupId, isGroupOwner }: CourseMaterialsProps) => {
  const [materials, setMaterials] = useState<CourseMaterial[]>([]);
  const [loading, setLoading] = useState(true);
  const [showUploadForm, setShowUploadForm] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const [uploadError, setUploadError] = useState<string | null>(null);
  const [selectedFile, setSelectedFile] = useState<File | null>(null);
  const [uploadForm, setUploadForm] = useState({
    title: '',
    description: '',
  });

  const token = localStorage.getItem('token');

  useEffect(() => {
    loadMaterials();
  }, [groupId]);

  const loadMaterials = async () => {
    setLoading(true);
    try {
      const response = await fetch(`http://localhost:5099/api/group/${groupId}/materials`, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setMaterials(data);
      }
    } catch (error) {
      console.error('Error loading materials:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleFileSelect = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files[0]) {
      setSelectedFile(e.target.files[0]);
      setUploadError(null);
    }
  };

  const handleUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFile) {
      setUploadError('Please select a file');
      return;
    }

    setIsUploading(true);
    setUploadError(null);

    const formData = new FormData();
    formData.append('file', selectedFile);
    formData.append('title', uploadForm.title || selectedFile.name);
    formData.append('description', uploadForm.description);

    try {
      const response = await fetch(`http://localhost:5099/api/group/${groupId}/materials`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
        body: formData,
      });

      if (response.ok) {
        await loadMaterials();
        setShowUploadForm(false);
        setUploadForm({ title: '', description: '' });
        setSelectedFile(null);
      } else {
        const error = await response.json();
        setUploadError(error.message || 'Failed to upload file');
      }
    } catch (error) {
      setUploadError('Network error. Please try again.');
    } finally {
      setIsUploading(false);
    }
  };

  const handleDelete = async (materialId: number) => {
    if (!window.confirm('Are you sure you want to delete this material?')) {
      return;
    }

    try {
      const response = await fetch(`http://localhost:5099/api/group/${groupId}/materials/${materialId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (response.ok) {
        await loadMaterials();
      }
    } catch (error) {
      console.error('Error deleting material:', error);
    }
  };

  const handleDownload = async (material: CourseMaterial) => {
    try {
      // Check if it's an external URL (starts with http:// or https://)
      if (material.fileUrl.startsWith('http://') || material.fileUrl.startsWith('https://')) {
        // For external URLs, open in new tab
        window.open(material.fileUrl, '_blank');
        return;
      }

      // For internal files, use fetch with authorization
      const response = await fetch(material.fileUrl, {
        headers: {
          'Authorization': `Bearer ${token}`,
        },
      });

      if (!response.ok) {
        throw new Error('Failed to download file');
      }

      const blob = await response.blob();
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement('a');
      link.href = url;
      link.download = material.fileName;
      document.body.appendChild(link);
      link.click();
      document.body.removeChild(link);
      window.URL.revokeObjectURL(url);
    } catch (error) {
      console.error('Error downloading file:', error);
      alert('Failed to download file. Please try again.');
    }
  };

  const getFileIcon = (fileType: string) => {
    if (fileType.includes('pdf')) return <FileText className="h-5 w-5" />;
    if (fileType.includes('video')) return <FileVideo className="h-5 w-5" />;
    if (fileType.includes('image')) return <FileImage className="h-5 w-5" />;
    if (fileType.includes('zip') || fileType.includes('rar')) return <FileArchive className="h-5 w-5" />;
    return <File className="h-5 w-5" />;
  };

  const getFileIconColor = (fileType: string) => {
    if (fileType.includes('pdf')) return 'text-red-600 bg-red-100 dark:bg-red-900/20';
    if (fileType.includes('video')) return 'text-purple-600 bg-purple-100 dark:bg-purple-900/20';
    if (fileType.includes('image')) return 'text-blue-600 bg-blue-100 dark:bg-blue-900/20';
    if (fileType.includes('zip') || fileType.includes('rar')) return 'text-orange-600 bg-orange-100 dark:bg-orange-900/20';
    return 'text-gray-600 bg-gray-100 dark:bg-gray-900/20';
  };

  const formatFileSize = (bytes: number) => {
    if (bytes < 1024) return bytes + ' B';
    if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(2) + ' KB';
    return (bytes / (1024 * 1024)).toFixed(2) + ' MB';
  };

  const formatDate = (date: string) => {
    return new Date(date).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
    });
  };

  return (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ delay: 0.6 }}
    >
      <Card>
        <CardHeader>
          <div className="flex items-center justify-between">
            <CardTitle className="flex items-center gap-2">
              <FileText className="h-5 w-5" />
              Course Materials ({materials.length})
            </CardTitle>
            {isGroupOwner && (
              <Button
                onClick={() => setShowUploadForm(!showUploadForm)}
                variant={showUploadForm ? 'outline' : 'default'}
              >
                {showUploadForm ? (
                  <>
                    <X className="h-4 w-4 mr-2" />
                    Cancel
                  </>
                ) : (
                  <>
                    <Upload className="h-4 w-4 mr-2" />
                    Upload Material
                  </>
                )}
              </Button>
            )}
          </div>
        </CardHeader>
        <CardContent>
          <AnimatePresence>
            {showUploadForm && (
              <motion.div
                initial={{ opacity: 0, height: 0 }}
                animate={{ opacity: 1, height: 'auto' }}
                exit={{ opacity: 0, height: 0 }}
                className="mb-6"
              >
                <Card className="bg-secondary/50">
                  <CardHeader>
                    <CardTitle className="text-lg">Upload New Material</CardTitle>
                  </CardHeader>
                  <CardContent>
                    <form onSubmit={handleUpload} className="space-y-4">
                      {uploadError && (
                        <motion.div
                          initial={{ opacity: 0, y: -10 }}
                          animate={{ opacity: 1, y: 0 }}
                          className="flex items-center gap-2 p-3 rounded-lg bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-900"
                        >
                          <AlertCircle className="h-4 w-4 text-red-600" />
                          <p className="text-sm text-red-600 dark:text-red-400">{uploadError}</p>
                        </motion.div>
                      )}

                      <div>
                        <label className="block text-sm font-medium mb-2">
                          File <span className="text-red-500">*</span>
                        </label>
                        <div className="relative">
                          <Input
                            type="file"
                            onChange={handleFileSelect}
                            required
                            className="cursor-pointer"
                          />
                          {selectedFile && (
                            <div className="mt-2 p-2 bg-blue-50 dark:bg-blue-900/20 rounded-md border border-blue-200 dark:border-blue-900">
                              <p className="text-sm text-blue-600 dark:text-blue-400 flex items-center gap-2">
                                <File className="h-4 w-4" />
                                {selectedFile.name} ({formatFileSize(selectedFile.size)})
                              </p>
                            </div>
                          )}
                        </div>
                      </div>

                      <div>
                        <label className="block text-sm font-medium mb-2">
                          Title
                        </label>
                        <Input
                          type="text"
                          placeholder="Enter material title (or use filename)"
                          value={uploadForm.title}
                          onChange={(e) => setUploadForm({ ...uploadForm, title: e.target.value })}
                        />
                      </div>

                      <div>
                        <label className="block text-sm font-medium mb-2">
                          Description
                        </label>
                        <textarea
                          placeholder="Enter material description..."
                          value={uploadForm.description}
                          onChange={(e) => setUploadForm({ ...uploadForm, description: e.target.value })}
                          rows={3}
                          className="w-full px-3 py-2 border border-input rounded-md bg-background focus:outline-none focus:ring-2 focus:ring-ring focus:border-transparent resize-y"
                        />
                      </div>

                      <Button
                        type="submit"
                        disabled={isUploading}
                        className="w-full"
                      >
                        {isUploading ? (
                          <>
                            <Loader2 className="h-4 w-4 mr-2 animate-spin" />
                            Uploading...
                          </>
                        ) : (
                          <>
                            <Upload className="h-4 w-4 mr-2" />
                            Upload Material
                          </>
                        )}
                      </Button>
                    </form>
                  </CardContent>
                </Card>
              </motion.div>
            )}
          </AnimatePresence>

          {loading ? (
            <div className="space-y-3">
              {[1, 2, 3].map((i) => (
                <div key={i} className="h-24 bg-secondary/50 rounded-lg animate-pulse" />
              ))}
            </div>
          ) : materials.length === 0 ? (
            <div className="text-center py-12">
              <FileText className="h-16 w-16 mx-auto text-muted-foreground/50 mb-4" />
              <p className="text-muted-foreground">No course materials yet</p>
              {isGroupOwner && (
                <p className="text-sm text-muted-foreground mt-2">
                  Click "Upload Material" to add the first file
                </p>
              )}
            </div>
          ) : (
            <div className="space-y-3">
              <AnimatePresence>
                {materials.map((material) => (
                  <motion.div
                    key={material.id}
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    exit={{ opacity: 0, y: -10 }}
                    transition={{ duration: 0.2 }}
                  >
                    <Card className="hover:shadow-md transition-shadow">
                      <CardContent className="pt-6">
                        <div className="flex items-start justify-between gap-4">
                          <div className="flex items-start gap-4 flex-1">
                            <div className={`p-3 rounded-xl ${getFileIconColor(material.fileType)}`}>
                              {getFileIcon(material.fileType)}
                            </div>
                            
                            <div className="flex-1 min-w-0">
                              <h4 className="font-semibold text-lg mb-1 truncate">
                                {material.title}
                              </h4>
                              
                              {material.description && (
                                <p className="text-muted-foreground text-sm mb-3 line-clamp-2">
                                  {material.description}
                                </p>
                              )}

                              <div className="flex flex-wrap gap-3 text-sm">
                                <div className="flex items-center gap-1.5 text-muted-foreground">
                                  <User className="h-4 w-4" />
                                  <span>{material.uploadedByProfessorName}</span>
                                </div>

                                <div className="flex items-center gap-1.5 text-muted-foreground">
                                  <Calendar className="h-4 w-4" />
                                  <span>{formatDate(material.uploadedAt)}</span>
                                </div>

                                <Badge variant="secondary" className="text-xs">
                                  {formatFileSize(material.fileSize)}
                                </Badge>
                              </div>
                            </div>
                          </div>

                          <div className="flex gap-2">
                            <Button
                              size="sm"
                              variant="outline"
                              onClick={() => handleDownload(material)}
                              className="hover:bg-blue-50 hover:text-blue-600 hover:border-blue-200 dark:hover:bg-blue-950/20"
                            >
                              <Download className="h-4 w-4" />
                            </Button>

                            {isGroupOwner && (
                              <Button
                                size="sm"
                                variant="outline"
                                onClick={() => handleDelete(material.id)}
                                className="hover:bg-red-50 hover:text-red-600 hover:border-red-200 dark:hover:bg-red-950/20"
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            )}
                          </div>
                        </div>
                      </CardContent>
                    </Card>
                  </motion.div>
                ))}
              </AnimatePresence>
            </div>
          )}
        </CardContent>
      </Card>
    </motion.div>
  );
};

export default CourseMaterials;
