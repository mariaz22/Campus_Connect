// src/data/floorplans/sociologie/parter.ts
import parterImg from "../../../assets/floorplans/sociologie/parter_sociologie.png";
import type { FloorPlanConfig } from "../type";

export const SAS_PARTER: FloorPlanConfig = {
  facultyKey: "sociologie",
  floorLabel: "Sociologie – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatru – Parter
    { roomName: "AmfSAS1", label: "AmfSAS1", x: 500, y: 300 },
  ],
};
