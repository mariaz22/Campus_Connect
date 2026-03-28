import etaj1Img from "../../../assets/floorplans/chimie/chimie_etaj1.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // TREBUIE să coincidă cu Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI (raportate la canvas)
  y: number;
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

export const CHIMIE_ETAJ1: FloorPlanConfig = {
  facultyKey: "chimie",
  floorLabel: "Chimie – Etaj 1",
  image: etaj1Img,

  // 🔒 FIX – identic cu parterul Chimie
  canvas: { w: 1000, h: 600 },

  pins: [
    // 🧪 Săli Etaj 1
    // ⚠️ coordonate orientative – le ajustezi fin din UI
    { roomName: "Ch101", label: "Ch101", x: 145, y: 210 },
    { roomName: "Ch102", label: "Ch102", x: 322, y: 210 },
    { roomName: "Ch103", label: "Ch103", x: 505, y: 210 },
    { roomName: "Ch104", label: "Ch104", x: 687, y: 210 },
    { roomName: "Ch105", label: "Ch105", x: 865, y: 210 },
  ],
};
