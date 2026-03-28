// src/data/floorplans/types.ts

export type Status = "Free" | "Occupied" | "OccupiedSoon" | string;

export type FloorPin = {
  roomName: string; // MUST match Room.Name din DB
  label?: string;
  x: number; // pixeli pe canvas
  y: number; // pixeli pe canvas
};

export type FloorPlanConfig = {
  facultyKey: string; 
  floorLabel: string;
  image: string;

  canvas: { w: number; h: number };
  pins: FloorPin[];
};
