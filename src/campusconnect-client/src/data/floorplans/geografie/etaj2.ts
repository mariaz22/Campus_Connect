// src/data/floorplans/geografie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/geografie/etaj2_geografie.png";
import type { FloorPlanConfig } from "../type";

export const GEO_ETAJ2: FloorPlanConfig = {
  facultyKey: "geografie",
  floorLabel: "Geografie – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Laboratoare și Seminarii – Etaj 2
    { roomName: "LabGeo201", label: "LabGeo201", x: 312, y: 225 },
    { roomName: "LabGeo202", label: "LabGeo202", x: 545, y: 225 },
    { roomName: "SemGeo203", label: "SemGeo203", x: 795, y: 225 },
  ],
};
