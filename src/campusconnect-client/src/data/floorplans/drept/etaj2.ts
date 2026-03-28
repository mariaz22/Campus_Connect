// src/data/floorplans/drept/etaj2.ts
import etaj2Img from "../../../assets/floorplans/drept/etaj2_drept.png";

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string;   // TREBUIE să coincidă cu Room.Name din DB
  label?: string;
  x: number;          // coordonate în PIXELI
  y: number;
};

export type FloorPlanConfig = {
  facultyKey: "drept";
  floorLabel: string;
  image: string;

  // 🔒 sistem de coordonate FIX (identic pt toate etajele Drept)
  canvas: {
    w: number;
    h: number;
  };

  pins: FloorPin[];
};

export const DREPT_ETAJ2: FloorPlanConfig = {
  facultyKey: "drept",
  floorLabel: "Drept – Etaj 2",
  image: etaj2Img,

  canvas: { w: 1000, h: 600 },

  pins: [
    // 📚 Seminarii – Etaj 2
    // ⚠️ coordonatele sunt orientative, le ajustezi fin din UI
    { roomName: "SemD201", label: "SemD201", x: 230, y: 240 },
    { roomName: "SemD202", label: "SemD202", x: 500, y: 240 },
    { roomName: "SemD203", label: "SemD203", x: 770, y: 240 },
  ],
};
