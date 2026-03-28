// src/data/floorplans/faa/etaj2.ts
import etaj2Img from "../../../assets/floorplans/faa/etaj2.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // MUST match Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI (pe canvas-ul fix)
  y: number;          // coordonate în PIXELI (pe canvas-ul fix)
};

export type FloorPlanConfig = {
  facultyKey: "faa";
  floorLabel: string;
  image: string;

  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const FAA_ETAJ2: FloorPlanConfig = {
  facultyKey: "faa",
  floorLabel: "FAA – Etaj 2",
  image: etaj2Img,

  // ⚠️ EXACT ca la etaj1.ts
  canvas: { w: 1000, h: 600 },

  pins: [
    // ✅ FAA Etaj 2 – săli din DB
    // TODO: pune coordonatele tale reale (acum sunt placeholder)
    { roomName: "S201", label: "S201", x: 200, y: 200 },
    { roomName: "S202", label: "S202", x: 505, y: 200 },
    { roomName: "S203", label: "S203", x: 800, y: 200 },
  ],
};
