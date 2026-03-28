// src/data/floorplans/fmi/etaj2.ts
import etaj2Img from "../../../assets/floorplans/fmi/etaj2.png";

// IMPORTANT: păstrăm aceleași tipuri ca în parter.ts ca să fie compatibil
export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string; // MUST match Room.Name din DB
  label?: string;
  x: number; // pixeli pe canvas
  y: number; // pixeli pe canvas
};

export type FloorPlanConfig = {
  facultyKey: "fmi";
  floorLabel: string;
  image: string;
  canvas: { w: number; h: number };
  pins: FloorPin[];
};

export const FMI_ETAJ2: FloorPlanConfig = {
  facultyKey: "fmi",
  floorLabel: "FMI – Etaj 2",
  image: etaj2Img,

  // ✅ dimensiuni reale ale imaginii tale (496 x 459)
  canvas: { w: 496, h: 459 },

  pins: [
    { roomName: "Amf. Dimitrie Pompeiu", label: "Amf. Pompeiu", x: 105, y: 242 },
    { roomName: "S201", label: "S201", x: 423, y: 144 },
  ],
};
