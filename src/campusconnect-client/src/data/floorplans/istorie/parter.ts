// src/data/floorplans/istorie/parter.ts
import parterImg from "../../../assets/floorplans/istorie/parter_istorie.png";
import type { FloorPlanConfig } from "../type";

export const IST_PARTER: FloorPlanConfig = {
  facultyKey: "istorie",
  floorLabel: "Istorie – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfIst1", label: "AmfIst1", x: 300, y: 300 },
    { roomName: "AmfIst2", label: "AmfIst2", x: 700, y: 300 },
  ],
};
