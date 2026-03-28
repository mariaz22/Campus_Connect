// src/data/floorplans/fmi/parter.ts
import parterImg from "../../../assets/floorplans/fmi/parter.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;
  label?: string;
  x: number; // ✅ pixeli pe canvas
  y: number; // ✅ pixeli pe canvas
};

export type FloorPlanConfig = {
  facultyKey: "fmi";
  floorLabel: string;
  image: string;

  canvas: { w: number; h: number }; // ✅ dimensiune fixă
  pins: FloorPin[];
};

export const FMI_PARTER: FloorPlanConfig = {
  facultyKey: "fmi",
  floorLabel: "FMI – Parter",
  image: parterImg,

  // 🔧 Alege o dimensiune “standard”
  // Recomand: 900x600 sau 1000x700
  canvas: { w: 532, h: 423 },

  pins: [
    { roomName: "Amf. Spiru Haret", label: "Amf. Haret", x: 117, y: 233 },
    { roomName: "S1", label: "S1", x: 233, y: 133 },
    { roomName: "S3", label: "S3", x: 377, y: 133 },
  ],
};
