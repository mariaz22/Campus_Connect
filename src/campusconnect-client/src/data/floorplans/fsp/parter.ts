// src/data/floorplans/fsp/parter.ts
import parterImg from "../../../assets/floorplans/fsp/parter_fsp.png";
import type { FloorPlanConfig } from "../type";

export const FSP_PARTER: FloorPlanConfig = {
  facultyKey: "fsp",
  floorLabel: "Științe Politice – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatru – Parter
    { roomName: "AmfFSP1", label: "AmfFSP1", x: 774, y: 335 },
  ],
};
