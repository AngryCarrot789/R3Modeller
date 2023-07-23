using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using OpenTK.Graphics.OpenGL;

namespace R3Modeller.Core.Engine.Collisions {
    public struct BoundingBox {
        public Vector3 min;
        public Vector3 max;

        public Vector3 Size => this.max - this.min;
        public Vector3 Scale => (this.max - this.min) / 2f;

        public BoundingBox(Vector3 min, Vector3 max) {
            this.min = min;
            this.max = max;
        }

        public BoundingBox(float minX, float minY, float minZ, float maxX, float maxY, float maxZ) {
            this.min = new Vector3(minX, minY, minZ);
            this.max = new Vector3(maxX, maxY, maxZ);
        }

        public BoundingBox(BoundingBox aabb) {
            this.min = aabb.min;
            this.max = aabb.max;
        }

        public static BoundingBox FromSafe(Vector3 min, Vector3 max) {
            return new BoundingBox(Vector3.Min(min, max), Vector3.Max(min, max));
        }

        public static BoundingBox FromSafe(float minX, float minY, float minZ, float maxX, float maxY, float maxZ) {
            return new BoundingBox(Math.Min(minX, maxX), Math.Min(minY, maxY), Math.Min(minZ, maxZ), Math.Max(minX, maxX), Math.Max(minY, maxY), Math.Max(minZ, maxZ));
        }

        public static BoundingBox FromSafe(BoundingBox aabb) {
            return new BoundingBox(Vector3.Min(aabb.min, aabb.max), Vector3.Max(aabb.min, aabb.max));
        }

        public static BoundingBox FromUniform(Vector3 center, Vector3 scale) {
            return new BoundingBox(center - scale, center + scale);
        }

        public BoundingBox Expand(float all) => this.Expand(new Vector3(all, all, all));
        public BoundingBox Expand(float x, float y, float z) => this.Expand(new Vector3(x, y, z));
        public BoundingBox Expand(Vector3 vec) => new BoundingBox(this.min - vec, this.max + vec);

        public BoundingBox Contract(float all) => this.Contract(new Vector3(all, all, all));
        public BoundingBox Contract(float x, float y, float z) => this.Contract(new Vector3(x, y, z));
        public BoundingBox Contract(Vector3 vec) => new BoundingBox(this.min + vec, this.max - vec);

        public BoundingBox Translate(float all) => this.Translate(new Vector3(all, all, all));
        public BoundingBox Translate(float x, float y, float z) => this.Translate(new Vector3(x, y, z));
        public BoundingBox Translate(Vector3 vec) => new BoundingBox(this.min + vec, this.max + vec);

        public bool Intersects(BoundingBox other) => this.Intersects(ref other);

        public bool Intersects(ref BoundingBox other) {
            return other.max.X >= this.min.X && other.min.X <= this.max.X &&
                   other.max.Y >= this.min.Y && other.min.Y <= this.max.Y &&
                   other.max.Z >= this.min.Z && other.min.Z <= this.max.Z;
        }

        public bool Intersects(Vector3 v) => this.Intersects(v.X, v.Y, v.Z);

        public bool Intersects(float x, float y, float z) {
            return x >= this.min.X && x <= this.max.X &&
                   y >= this.min.Y && y <= this.max.Y &&
                   z >= this.min.Z && z <= this.max.Z;
        }

        public override string ToString() {
            return $"[{this.min.X:F2},{this.min.Y:F2},{this.min.Z:F2} -> {this.max.X:F2},{this.max.Y:F2},{this.max.Z:F2}] ({this.max - this.min})";
        }
    }
}