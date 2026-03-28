// src/data/floorplans/faa/etaj1.ts
import etaj1Img from "../../../assets/floorplans/faa/etaj1.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // MUST match Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI
  y: number;          // coordonate în PIXELI
};

export type FloorPlanConfig = {
  facultyKey: "faa";
  floorLabel: string;
  image: string;

  // ⚠️ foarte important: canvas FIX
  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const FAA_ETAJ1: FloorPlanConfig = {
  facultyKey: "faa",
  floorLabel: "FAA – Etaj 1",
  image: etaj1Img,

  // 🔧 AICI stabilești „sistemul de coordonate”
  // Folosește exact aceleași valori pentru TOATE etajele FAA
  canvas: { w: 1000, h: 600 },

  pins: [
    // 🔵 Etaj 1 – săli
    { roomName: "A101", label: "A101", x: 188, y: 192 },
    { roomName: "A102", label: "A102", x: 405, y: 192 },
    { roomName: "A103", label: "A103", x: 626, y: 192 },
    { roomName: "A104", label: "A104", x: 816, y: 192 },
    { roomName: "A105", label: "A105", x: 618, y: 329 },
  ],
};
