// src/data/floorplans/geologie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/geologie/etaj2_geologie.png";
import type { FloorPlanConfig } from "../type";

export const GG_ETAJ2: FloorPlanConfig = {
  facultyKey: "geologie",
  floorLabel: "Geologie și Geofizică – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Laboratoare și Seminarii – Etaj 2
    { roomName: "LabGG201", label: "LabGG201", x: 313, y: 215 },
    { roomName: "LabGG202", label: "LabGG202", x: 557, y: 215 },
    { roomName: "SemGG203", label: "SemGG203", x: 805, y: 215 },
  ],
};
