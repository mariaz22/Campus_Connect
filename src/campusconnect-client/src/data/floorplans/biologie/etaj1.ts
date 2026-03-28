// src/data/floorplans/bio/etaj1.ts
import etaj1Img from "../../../assets/floorplans/biologie/etaj1_bio.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // MUST match Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI (raportate la canvas)
  y: number;          // coordonate în PIXELI (raportate la canvas)
};

export type FloorPlanConfig = {
  facultyKey: string; // ✅ IMPORTANT (ca să fie compatibil cu FMI/FAA/BIO)
  floorLabel: string;
  image: string;

  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const BIO_ETAJ1: FloorPlanConfig = {
  facultyKey: "bio",
  floorLabel: "Biologie – Etaj 1",
  image: etaj1Img,

  // ⚠️ Păstrează același canvas pentru TOATE etajele Bio
  canvas: { w: 1000, h: 700 },

  pins: [
    // 🔵 Etaj 1 – săli (coordonate orientative, le ajustezi după imagine)
    { roomName: "Bio101", label: "Bio101", x: 485, y: 262 },
    { roomName: "Bio102", label: "Bio102", x: 778, y: 262 },
    { roomName: "Bio103", label: "Bio103", x: 211, y: 503 },
    { roomName: "Bio104", label: "Bio104", x: 482, y: 503 },
    { roomName: "Bio105", label: "Bio105", x: 770, y: 503 },
  ],
};
