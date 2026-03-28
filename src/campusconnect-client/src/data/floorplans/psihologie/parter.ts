// src/data/floorplans/psihologie/parter.ts
import parterImg from "../../../assets/floorplans/psihologie/parter_psihologie.png";
import type { FloorPlanConfig } from "../type";

export const PSI_PARTER: FloorPlanConfig = {
  facultyKey: "psihologie",
  floorLabel: "Psihologie – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // Amfiteatre – Parter
    { roomName: "AmfPsi1", label: "AmfPsi1", x: 300, y: 330 },
    { roomName: "AmfPsi2", label: "AmfPsi2", x: 700, y: 330 },
  ],
};
