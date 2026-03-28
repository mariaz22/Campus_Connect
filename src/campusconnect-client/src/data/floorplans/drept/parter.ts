// src/data/floorplans/drept/parter.ts
import parterImg from "../../../assets/floorplans/drept/parter_drept.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string; // MUST match Room.Name din DB
  label?: string;
  x: number; // coordonate în PIXELI (pe canvas-ul fix)
  y: number; // coordonate în PIXELI (pe canvas-ul fix)
};

export type FloorPlanConfig = {
  facultyKey: "drept";
  floorLabel: string;
  image: string;

  // IMPORTANT: canvas FIX (folosește aceleași valori și la etaje Drept viitoare)
  canvas: { w: number; h: number };

  pins: FloorPin[];
};

export const DREPT_PARTER: FloorPlanConfig = {
  facultyKey: "drept",
  floorLabel: "Drept – Parter",
  image: parterImg,

  // Recomand să păstrezi 1000x600 ca la multe din schițele tale
  canvas: { w: 1000, h: 600 },

  pins: [
    // 🔧 coordonatele sunt orientative — le ajustezi cu click până se potrivesc
    { roomName: "AmfD1", label: "AmfD1", x: 242, y: 262 },
    { roomName: "AmfD2", label: "AmfD2", x: 750, y: 262 },
  ],
};
