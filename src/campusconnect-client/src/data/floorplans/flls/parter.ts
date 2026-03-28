// src/data/floorplans/flls/parter.ts
import parterImg from "../../../assets/floorplans/flls/parter_flls.png";
import type { FloorPlanConfig } from "../type";

export const FLLS_PARTER: FloorPlanConfig = {
  facultyKey: "flls",
  floorLabel: "FLLS – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfLLS1", label: "AmfLLS1", x: 300, y: 300 },
    { roomName: "AmfLLS2", label: "AmfLLS2", x: 700, y: 300 },
  ],
};
