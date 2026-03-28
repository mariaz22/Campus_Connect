// src/data/floorplans/fizica/parter.ts
import parterImg from "../../../assets/floorplans/fizica/parter_fizica.png";
import type { FloorPlanConfig } from "../type";

export const FIZ_PARTER: FloorPlanConfig = {
  facultyKey: "fizica",
  floorLabel: "Fizică – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfFiz1", label: "AmfFiz1", x: 338, y: 300 },
    { roomName: "AmfFiz2", label: "AmfFiz2", x: 700, y: 300 },
  ],
};
