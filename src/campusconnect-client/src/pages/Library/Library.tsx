import { useEffect, useMemo, useRef, useState } from 'react';
import { motion, AnimatePresence } from 'framer-motion';
import {
  FolderPlus,
  Folder,
  FileText,
  Link as LinkIcon,
  Upload,
  Trash2,
  Sparkles,
  Download,
  Search,
  X
} from 'lucide-react';

import { Layout } from '../../components/Layout';
import { Card, CardHeader, CardTitle, CardContent } from '../../components/ui/Card';
import { Button } from '../../components/ui/Button';
import { Input } from '../../components/ui/Input';
import { Badge } from '../../components/ui/Badge';
import { Skeleton } from '../../components/ui/Skeleton';

import { libraryApi, type LibraryFolder, type LibraryItem } from '../../services/libraryApi';

function isFileType(t: number | string) {
  if (typeof t === 'number') return t === 1;
  return String(t).toLowerCase() === 'file';
}
function isLinkType(t: number | string) {
  if (typeof t === 'number') return t === 2;
  return String(t).toLowerCase() === 'link';
}

export default function Library() {
  const user = JSON.parse(localStorage.getItem('user') || '{}');
  const currentUserId = (user.id ?? user.userId ?? localStorage.getItem('userId') ?? '').toString();
  const isAdmin = user.role === 'Admin';

  const [folders, setFolders] = useState<LibraryFolder[]>([]);
  const [selectedFolder, setSelectedFolder] = useState<LibraryFolder | null>(null);
  const [items, setItems] = useState<LibraryItem[]>([]);

  const [loadingFolders, setLoadingFolders] = useState(true);
  const [loadingItems, setLoadingItems] = useState(false);

  const [showCreateFolder, setShowCreateFolder] = useState(false);
  const [newFolderName, setNewFolderName] = useState('');

  const [showAddLink, setShowAddLink] = useState(false);
  const [linkTitle, setLinkTitle] = useState('');
  const [linkUrl, setLinkUrl] = useState('');

  const [showUpload, setShowUpload] = useState(false);
  const [uploadTitle, setUploadTitle] = useState('');
  const [uploadFile, setUploadFile] = useState<File | null>(null);

  // ✅ Search pt items (în folder)
  const [search, setSearch] = useState('');
  const [typeFilter, setTypeFilter] = useState<'all' | 'file' | 'link'>('all');

  // ✅ Search pt foldere (pagina principală)
  const [folderSearch, setFolderSearch] = useState('');

  // ✅ în loc de alert() – error state în UI
  const [error, setError] = useState<string | null>(null);

  // ✅ protecție pentru StrictMode (dev) ca să nu ruleze useEffect de 2 ori
  const didInit = useRef(false);

  useEffect(() => {
    if (didInit.current) return;
    didInit.current = true;
    loadFolders();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const loadFolders = async () => {
    setLoadingFolders(true);
    setError(null);
    try {
      const data = await libraryApi.getFolders();
      setFolders(data);
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to fetch folders.');
    } finally {
      setLoadingFolders(false);
    }
  };

  const loadItems = async (folder: LibraryFolder) => {
    setSelectedFolder(folder);
    setLoadingItems(true);
    setError(null);

    setSearch('');
    setTypeFilter('all');
    setShowAddLink(false);
    setShowUpload(false);

    try {
      const data = await libraryApi.getItems(folder.id);
      setItems(data);
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to fetch items.');
    } finally {
      setLoadingItems(false);
    }
  };

  const filteredItems = useMemo(() => {
    const q = search.trim().toLowerCase();
    return items.filter((it) => {
      const matchesText =
        !q ||
        it.title.toLowerCase().includes(q) ||
        (it.originalFileName ?? '').toLowerCase().includes(q) ||
        (it.url ?? '').toLowerCase().includes(q);

      const matchesType =
        typeFilter === 'all' ||
        (typeFilter === 'file' && isFileType(it.type)) ||
        (typeFilter === 'link' && isLinkType(it.type));

      return matchesText && matchesType;
    });
  }, [items, search, typeFilter]);

  const filteredFolders = useMemo(() => {
    const q = folderSearch.trim().toLowerCase();
    if (!q) return folders;
    return folders.filter((f) => f.name.toLowerCase().includes(q));
  }, [folders, folderSearch]);

  const canDeleteItem = (it: LibraryItem) => {
    if (isAdmin) return true;
    return it.createdByUserId === currentUserId;
  };

  const handleCreateFolder = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newFolderName.trim()) return;

    setError(null);
    try {
      await libraryApi.createFolder({ name: newFolderName.trim() });
      setNewFolderName('');
      setShowCreateFolder(false);
      await loadFolders();
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to create folder.');
    }
  };

  const handleDeleteFolder = async (folderId: string) => {
    if (!isAdmin) return;
    if (!window.confirm('Sigur vrei să ștergi folderul? (se șterg și materialele)')) return;

    setError(null);
    try {
      await libraryApi.deleteFolder(folderId);
      if (selectedFolder?.id === folderId) {
        setSelectedFolder(null);
        setItems([]);
      }
      await loadFolders();
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to delete folder.');
    }
  };

  const handleAddLink = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFolder) return;

    setError(null);
    try {
      await libraryApi.addLink(selectedFolder.id, {
        title: linkTitle.trim(),
        url: linkUrl.trim()
      });
      setLinkTitle('');
      setLinkUrl('');
      setShowAddLink(false);
      await loadItems(selectedFolder);
      await loadFolders();
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to add link.');
    }
  };

  const handleUpload = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!selectedFolder) return;
    if (!uploadFile) return;

    setError(null);
    try {
      await libraryApi.uploadFile(selectedFolder.id, uploadTitle.trim(), uploadFile);
      setUploadTitle('');
      setUploadFile(null);
      setShowUpload(false);
      await loadItems(selectedFolder);
      await loadFolders();
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to upload file.');
    }
  };

  const handleDeleteItem = async (itemId: string) => {
    if (!window.confirm('Sigur vrei să ștergi materialul?')) return;

    setError(null);
    try {
      await libraryApi.deleteItem(itemId);
      if (selectedFolder) await loadItems(selectedFolder);
      await loadFolders();
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to delete item.');
    }
  };

  const handleDownload = async (it: LibraryItem) => {
    setError(null);
    try {
      const blob = await libraryApi.downloadItem(it.id);
      const url = URL.createObjectURL(blob);

      const a = document.createElement('a');
      a.href = url;
      a.download = it.originalFileName || it.title || 'download';
      document.body.appendChild(a);
      a.click();
      a.remove();

      URL.revokeObjectURL(url);
    } catch (e) {
      console.error(e);
      setError((e as Error).message || 'Failed to download file.');
    }
  };

  return (
    <Layout>
      <div className="space-y-6">
        {/* Header */}
        <motion.div
          initial={{ opacity: 0, y: 20 }}
          animate={{ opacity: 1, y: 0 }}
          className="relative overflow-hidden rounded-2xl bg-gradient-to-br from-rose-600 via-pink-600 to-fuchsia-700 p-8 text-white shadow-2xl"
        >
          <div className="flex items-center gap-3">
            <div className="p-3 bg-white/15 backdrop-blur-sm rounded-xl border border-white/20">
              <Folder className="h-8 w-8 text-white" />
            </div>
            <div>
              <h1 className="text-4xl font-bold flex items-center gap-2">
                Library <Sparkles className="h-6 w-6 text-yellow-300" />
              </h1>
              <p className="text-white/85 mt-1">Course folders and materials</p>
            </div>
          </div>
        </motion.div>

        {/* Error banner */}
        {error && (
          <div className="rounded-xl border border-red-200 bg-red-50 px-4 py-3 text-sm text-red-700">
            <div className="flex items-start justify-between gap-3">
              <div className="min-w-0">
                <p className="font-semibold">A apărut o problemă</p>
                <p className="break-words">{error}</p>
              </div>
              <Button variant="outline" onClick={() => setError(null)} className="shrink-0">
                <X className="h-4 w-4 mr-2" /> Close
              </Button>
            </div>
          </div>
        )}

        {/* Folders view */}
        {!selectedFolder && (
          <>
            {/* ✅ Search + counter + New Folder aliniate pe aceeași linie */}
            <div className="flex items-start justify-between gap-3">
              <div className="flex-1">
                <div className="relative">
                  <Search className="h-4 w-4 absolute left-3 top-3 text-muted-foreground" />
                  <Input
                    value={folderSearch}
                    onChange={(e) => setFolderSearch(e.target.value)}
                    placeholder="Search..."
                    className="pl-9"
                  />
                </div>

                <div className="text-sm text-muted-foreground mt-2">
                  {loadingFolders ? 'Loading...' : `${filteredFolders.length} folders`}
                </div>
              </div>

              <div className="shrink-0">
                <Button
                  variant={showCreateFolder ? 'outline' : 'default'}
                  onClick={() => setShowCreateFolder(!showCreateFolder)}
                >
                  {showCreateFolder ? (
                    <>
                      <X className="h-4 w-4 mr-2" />
                      Cancel
                    </>
                  ) : (
                    <>
                      <FolderPlus className="h-4 w-4 mr-2" />
                      New Folder
                    </>
                  )}
                </Button>
              </div>
            </div>

            <AnimatePresence>
              {showCreateFolder && (
                <motion.div
                  initial={{ opacity: 0, height: 0 }}
                  animate={{ opacity: 1, height: 'auto' }}
                  exit={{ opacity: 0, height: 0 }}
                >
                  <Card>
                    <CardHeader>
                      <CardTitle>Create Folder</CardTitle>
                    </CardHeader>
                    <CardContent>
                      <form onSubmit={handleCreateFolder} className="flex flex-col md:flex-row gap-3">
                        <Input
                          value={newFolderName}
                          onChange={(e) => setNewFolderName(e.target.value)}
                          placeholder="ex: Matematică, POO, Baze de Date..."
                          required
                        />
                        <Button type="submit" className="md:w-48">
                          Create
                        </Button>
                      </form>
                    </CardContent>
                  </Card>
                </motion.div>
              )}
            </AnimatePresence>

            {loadingFolders ? (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                {Array.from({ length: 6 }).map((_, i) => (
                  <Skeleton key={i} className="h-32 w-full rounded-2xl" />
                ))}
              </div>
            ) : filteredFolders.length === 0 ? (
              <div className="text-center py-12">
                <Folder className="h-16 w-16 mx-auto text-muted-foreground/50 mb-4" />
                <h3 className="text-xl font-semibold mb-2">No folders found</h3>
                <p className="text-muted-foreground">
                  {folderSearch.trim()
                    ? 'Nu există niciun folder care să se potrivească căutării.'
                    : 'Creează primul folder și începe să adaugi materiale.'}
                </p>
              </div>
            ) : (
              <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
                <AnimatePresence mode="popLayout">
                  {filteredFolders.map((f, index) => (
                    <motion.div
                      key={f.id}
                      initial={{ opacity: 0, scale: 0.95 }}
                      animate={{ opacity: 1, scale: 1 }}
                      exit={{ opacity: 0, scale: 0.95 }}
                      transition={{ delay: index * 0.03 }}
                      layout
                    >
                      <Card className="group relative overflow-hidden border-2 hover:border-primary/50 transition-all h-full hover:shadow-xl">
                        <div className="pointer-events-none absolute inset-0 bg-gradient-to-br from-primary/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity"></div>

                        <CardHeader className="relative">
                          <div className="flex items-start justify-between gap-3">
                            <CardTitle className="text-xl line-clamp-2 group-hover:text-primary transition-colors">
                              {f.name}
                            </CardTitle>

                            {/* ✅ aceeași culoare ca butonul default */}
                            <Badge className="bg-primary text-primary-foreground border-0 whitespace-nowrap">
                              {(f.itemsCount ?? 0)} items
                            </Badge>
                          </div>
                        </CardHeader>

                        <CardContent className="relative">
                          <div className="flex gap-2">
                            <Button variant="outline" className="w-full" onClick={() => loadItems(f)}>
                              <Folder className="h-4 w-4 mr-2" />
                              Open
                            </Button>

                            {isAdmin && (
                              <Button
                                variant="outline"
                                className="hover:bg-red-50 hover:text-red-600 hover:border-red-200"
                                onClick={() => handleDeleteFolder(f.id)}
                                title="Admin only"
                              >
                                <Trash2 className="h-4 w-4" />
                              </Button>
                            )}
                          </div>
                        </CardContent>
                      </Card>
                    </motion.div>
                  ))}
                </AnimatePresence>
              </div>
            )}
          </>
        )}

        {/* Items view */}
        {selectedFolder && (
          <div className="space-y-4">
            <Card>
              <CardHeader>
                <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-3">
                  <div>
                    <CardTitle className="flex items-center gap-2">
                      <Folder className="h-5 w-5" />
                      {selectedFolder.name}
                    </CardTitle>
                    <div className="text-sm text-muted-foreground mt-1">
                      {loadingItems ? 'Loading...' : `${items.length} materials`}
                    </div>
                  </div>

                  <div className="flex flex-wrap gap-2">
                    <Button
                      variant={showAddLink ? 'outline' : 'default'}
                      onClick={() => (setShowAddLink(!showAddLink), setShowUpload(false))}
                    >
                      <LinkIcon className="h-4 w-4 mr-2" />
                      Add Link
                    </Button>
                    <Button
                      variant={showUpload ? 'outline' : 'default'}
                      onClick={() => (setShowUpload(!showUpload), setShowAddLink(false))}
                    >
                      <Upload className="h-4 w-4 mr-2" />
                      Upload File
                    </Button>

                    <Button
                      variant="outline"
                      onClick={() => {
                        setSelectedFolder(null);
                        setItems([]);
                        setShowAddLink(false);
                        setShowUpload(false);
                        setSearch('');
                        setTypeFilter('all');
                        setError(null);
                      }}
                    >
                      <X className="h-4 w-4 mr-2" />
                      Close Folder
                    </Button>
                  </div>
                </div>
              </CardHeader>

              <CardContent className="space-y-4">
                <div className="flex flex-col md:flex-row gap-3">
                  <div className="relative flex-1">
                    <Search className="h-4 w-4 absolute left-3 top-3 text-muted-foreground" />
                    <Input
                      value={search}
                      onChange={(e) => setSearch(e.target.value)}
                      placeholder="Search by title, filename or URL..."
                      className="pl-9"
                    />
                  </div>

                  <div className="flex gap-2">
                    <Button variant={typeFilter === 'all' ? 'default' : 'outline'} onClick={() => setTypeFilter('all')}>
                      All
                    </Button>
                    <Button variant={typeFilter === 'file' ? 'default' : 'outline'} onClick={() => setTypeFilter('file')}>
                      Files
                    </Button>
                    <Button variant={typeFilter === 'link' ? 'default' : 'outline'} onClick={() => setTypeFilter('link')}>
                      Links
                    </Button>
                  </div>
                </div>

                <AnimatePresence>
                  {showAddLink && (
                    <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }}>
                      <Card className="bg-secondary/50">
                        <CardHeader>
                          <CardTitle className="text-lg flex items-center gap-2">
                            <LinkIcon className="h-4 w-4" /> Add Link
                          </CardTitle>
                        </CardHeader>
                        <CardContent>
                          <form onSubmit={handleAddLink} className="space-y-3">
                            <Input value={linkTitle} onChange={(e) => setLinkTitle(e.target.value)} placeholder="Title (ex: Curs 1)" required />
                            <Input value={linkUrl} onChange={(e) => setLinkUrl(e.target.value)} placeholder="https://..." required />
                            <Button type="submit" className="w-full">
                              Save Link
                            </Button>
                          </form>
                        </CardContent>
                      </Card>
                    </motion.div>
                  )}
                </AnimatePresence>

                <AnimatePresence>
                  {showUpload && (
                    <motion.div initial={{ opacity: 0, height: 0 }} animate={{ opacity: 1, height: 'auto' }} exit={{ opacity: 0, height: 0 }}>
                      <Card className="bg-secondary/50">
                        <CardHeader>
                          <CardTitle className="text-lg flex items-center gap-2">
                            <Upload className="h-4 w-4" /> Upload File
                          </CardTitle>
                        </CardHeader>
                        <CardContent>
                          <form onSubmit={handleUpload} className="space-y-3">
                            <Input value={uploadTitle} onChange={(e) => setUploadTitle(e.target.value)} placeholder="Title (ex: Seminar 2)" required />
                            <input type="file" onChange={(e) => setUploadFile(e.target.files?.[0] ?? null)} className="block w-full text-sm" />
                            <Button type="submit" className="w-full" disabled={!uploadFile}>
                              Upload
                            </Button>
                            <p className="text-xs text-muted-foreground">Max 50MB. Tipuri permise: pdf, doc(x), ppt(x), xls(x), png/jpg.</p>
                          </form>
                        </CardContent>
                      </Card>
                    </motion.div>
                  )}
                </AnimatePresence>

                {loadingItems ? (
                  <div className="space-y-3">
                    {Array.from({ length: 5 }).map((_, i) => (
                      <Skeleton key={i} className="h-20 w-full rounded-xl" />
                    ))}
                  </div>
                ) : filteredItems.length === 0 ? (
                  <div className="text-center py-10">
                    <FileText className="h-14 w-14 mx-auto text-muted-foreground/50 mb-3" />
                    <p className="text-muted-foreground">No materials found.</p>
                  </div>
                ) : (
                  <div className="space-y-3">
                    <AnimatePresence>
                      {filteredItems.map((it) => (
                        <motion.div key={it.id} initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
                          <Card className="hover:shadow-lg transition-shadow">
                            <CardContent className="pt-6">
                              <div className="flex items-start justify-between gap-4">
                                <div className="min-w-0 flex-1">
                                  <div className="flex items-center gap-2 mb-1">
                                    {isFileType(it.type) ? (
                                      <Badge className="bg-blue-500 text-white border-0">FILE</Badge>
                                    ) : (
                                      <Badge className="bg-emerald-500 text-white border-0">LINK</Badge>
                                    )}
                                    <h4 className="font-semibold text-lg truncate">{it.title}</h4>
                                  </div>

                                  {isLinkType(it.type) && it.url && (
                                    <a href={it.url} target="_blank" rel="noreferrer" className="text-sm text-blue-600 hover:underline break-all">
                                      {it.url}
                                    </a>
                                  )}

                                  {isFileType(it.type) && (
                                    <p className="text-sm text-muted-foreground">
                                      {it.originalFileName || it.contentType || 'File'}
                                      {typeof it.sizeBytes === 'number' ? ` • ${(it.sizeBytes / (1024 * 1024)).toFixed(2)} MB` : ''}
                                    </p>
                                  )}
                                </div>

                                <div className="flex flex-col sm:flex-row gap-2">
                                  {isFileType(it.type) && (
                                    <Button variant="outline" onClick={() => handleDownload(it)}>
                                      <Download className="h-4 w-4 mr-2" />
                                      Download
                                    </Button>
                                  )}

                                  {canDeleteItem(it) && (
                                    <Button
                                      variant="outline"
                                      className="hover:bg-red-50 hover:text-red-600 hover:border-red-200"
                                      onClick={() => handleDeleteItem(it.id)}
                                    >
                                      <Trash2 className="h-4 w-4 mr-2" />
                                      Delete
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
          </div>
        )}
      </div>
    </Layout>
  );
}
