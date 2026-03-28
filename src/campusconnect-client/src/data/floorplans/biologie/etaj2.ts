// src/data/floorplans/bio/etaj2.ts
import etaj2Img from "../../../assets/floorplans/biologie/etaj2_bio.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // MUST match Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI
  y: number;          // coordonate în PIXELI
};

export type FloorPlanConfig = {
  facultyKey: "bio";
  floorLabel: string;
  image: string;

  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const BIO_ETAJ2: FloorPlanConfig = {
  facultyKey: "bio",
  floorLabel: "Biologie – Etaj 2",
  image: etaj2Img,

  // 🔒 FIX – la fel pentru toate etajele Bio
  canvas: { w: 1000, h: 600 },

  pins: [
    // 🧪 Laboratoare
    { roomName: "LabBio201", label: "LabBio201", x: 181, y: 428 },
    { roomName: "LabBio202", label: "LabBio202", x: 497, y: 428 },
    { roomName: "LabBio203", label: "LabBio203", x: 790, y: 428 },
  ],
};
