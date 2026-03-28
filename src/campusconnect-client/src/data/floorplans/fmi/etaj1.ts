// src/data/floorplans/fmi/etaj1.ts
import etaj1Img from "../../../assets/floorplans/fmi/et1.png";

// IMPORTANT: păstrăm aceleași tipuri ca în parter.ts ca să fie compatibil
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

  canvas: { w: number; h: number }; 
  pins: FloorPin[];
};

export const FMI_ETAJ1: FloorPlanConfig = {
  facultyKey: "fmi",
  floorLabel: "FMI – Etaj 1",
  image: etaj1Img,

  canvas: { w: 520, h: 461 },

  pins: [
    { roomName: "Amf. Simion Stoilow", label: "Amf. Stoilow", x: 110, y: 247 },
    { roomName: "Lab FMI 1", label: "Lab 1", x: 228, y: 141 },
    { roomName: "Lab FMI 2", label: "Lab 2", x: 300, y: 141 },
    { roomName: "Lab FMI 3", label: "Lab 3", x: 370, y: 141 },
    { roomName: "S101", label: "S101", x: 228, y: 299 },
    { roomName: "S102", label: "S102", x: 300, y: 299 },
    { roomName: "S103", label: "S103", x: 370, y: 299 },
  ],
};
