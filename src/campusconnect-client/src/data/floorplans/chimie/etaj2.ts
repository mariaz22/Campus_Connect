// src/data/floorplans/chimie/etaj2.ts
import etaj2Img from "../../../assets/floorplans/chimie/chimie_etaj2.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // MUST match Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI
  y: number;          // coordonate în PIXELI
};

export type FloorPlanConfig = {
  facultyKey: "chimie";
  floorLabel: string;
  image: string;

  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const CHIMIE_ETAJ2: FloorPlanConfig = {
  facultyKey: "chimie",
  floorLabel: "Chimie – Etaj 2",
  image: etaj2Img,

  // 🔒 FIX – păstrează la fel pentru TOATE etajele Chimie
  canvas: { w: 1000, h: 600 },

  pins: [
    // 🧪 Laboratoare (Etaj 2)
    // NOTE: coordonate EXEMPLE — le ajustezi tu
    { roomName: "LabCh201", label: "LabCh201", x: 187, y: 259 },
    { roomName: "LabCh202", label: "LabCh202", x: 485, y: 259 },
    { roomName: "LabCh203", label: "LabCh203", x: 783, y: 259 },
  ],
};
