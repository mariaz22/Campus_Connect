// src/data/floorplans/bio/parter.ts
import parterImg from "../../../assets/floorplans/biologie/parter_bio.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // TREBUIE să coincidă cu Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI
  y: number;          // coordonate în PIXELI
};

export type FloorPlanConfig = {
  facultyKey: "bio";
  floorLabel: string;
  image: string;

  // ⚠️ IMPORTANT: canvas FIX
  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const BIO_PARTER: FloorPlanConfig = {
  facultyKey: "bio",
  floorLabel: "Biologie – Parter",
  image: parterImg,

  // 👉 alege valori clare și păstrează-le la TOATE etajele Bio
  canvas: { w: 1000, h: 700 },

  pins: [
    // 🔵 Amfiteatre – Parter
    { roomName: "AmfBio1", label: "Amf. Bio 1", x: 240, y: 442 },
    { roomName: "AmfBio2", label: "Amf. Bio 2", x: 722, y: 442 },
  ],
};
