// src/data/floorplans/geologie/parter.ts
import parterImg from "../../../assets/floorplans/geologie/parter_geologie.png";
import type { FloorPlanConfig } from "../type";

export const GG_PARTER: FloorPlanConfig = {
  facultyKey: "geologie",
  floorLabel: "Geologie și Geofizică – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfGG1", label: "AmfGG1", x: 341, y: 300 },
    { roomName: "AmfGG2", label: "AmfGG2", x: 700, y: 300 },
  ],
};
