// src/data/floorplans/litere/parter.ts
import parterImg from "../../../assets/floorplans/litere/parter_litere.png";
import type { FloorPlanConfig } from "../type";

export const LIT_PARTER: FloorPlanConfig = {
  facultyKey: "litere",
  floorLabel: "Litere – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfLit1", label: "AmfLit1", x: 300, y: 300 },
    { roomName: "AmfLit2", label: "AmfLit2", x: 700, y: 300 },
  ],
};
