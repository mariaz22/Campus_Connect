import parterImg from "../../../assets/floorplans/chimie/chimie_parter.png";

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

export const CHIMIE_PARTER: FloorPlanConfig = {
  facultyKey: "chimie",
  floorLabel: "Chimie – Parter",
  image: parterImg,

  canvas: { w: 1000, h: 600 },

  pins: [
    // 🧑‍🏫 Amfiteatre (parter)
    // IMPORTANT: coordonatele sunt EXEMPLE — le ajustezi tu cu click până se potrivesc perfect
    { roomName: "AmfCh1", label: "AmfCh1", x: 320, y: 440 },
    { roomName: "AmfCh2", label: "AmfCh2", x: 720, y: 440 },
  ],
};
