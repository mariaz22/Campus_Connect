// src/data/floorplans/drept/etaj1.ts
import etaj1Img from "../../../assets/floorplans/drept/etaj1_drept.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // MUST match Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI
  y: number;          // coordonate în PIXELI
};

export type FloorPlanConfig = {
  facultyKey: "drept";
  floorLabel: string;
  image: string;

  // 🔒 FIX – IDENTIC cu parterul Drept
  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const DREPT_ETAJ1: FloorPlanConfig = {
  facultyKey: "drept",
  floorLabel: "Drept – Etaj 1",
  image: etaj1Img,

  // ⚠️ FOARTE IMPORTANT
  // aceeași dimensiune la TOATE etajele Drept
  canvas: { w: 1000, h: 600 },

  pins: [
    // 🏫 Săli Etaj 1
    { roomName: "D101", label: "D101", x: 143, y: 233 },
    { roomName: "D102", label: "D102", x: 320, y: 234 },
    { roomName: "D103", label: "D103", x: 500, y: 231 },
    { roomName: "D104", label: "D104", x: 680, y: 231 },
    { roomName: "D105", label: "D105", x: 861, y: 231 },
  ],
};
