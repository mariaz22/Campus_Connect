// src/data/floorplans/filosofie/parter.ts
import parterImg from "../../../assets/floorplans/filosofie/parter_filo.png";
import type { FloorPlanConfig } from "../type";

export const FILO_PARTER: FloorPlanConfig = {
  facultyKey: "filosofie",
  floorLabel: "Filosofie – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfFilo1", label: "AmfFilo1", x: 313, y: 334 },
    { roomName: "AmfFilo2", label: "AmfFilo2", x: 711, y: 334 },
  ],
};
