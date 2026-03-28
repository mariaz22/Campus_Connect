// src/data/floorplans/geografie/parter.ts
import parterImg from "../../../assets/floorplans/geografie/parter_geografie.png";
import type { FloorPlanConfig } from "../type";

export const GEO_PARTER: FloorPlanConfig = {
  facultyKey: "geografie",
  floorLabel: "Geografie – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfGeo1", label: "AmfGeo1", x: 336, y: 300 },
    { roomName: "AmfGeo2", label: "AmfGeo2", x: 700, y: 300 },
  ],
};
