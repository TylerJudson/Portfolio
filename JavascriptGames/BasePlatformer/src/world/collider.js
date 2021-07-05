export class Collider {
    constructor(position, radius) {
        this.position = position;
        this.radius = radius;
    }
    update(position) {
        this.position = position;
    }
    didCollide(collider) {
        if (this.broadCollision(collider)) {
            return this.narrowCollision(collider);
        }
        return false;
    }
    broadCollision(collider) {
        // Basic Circle Detection Collision
        let Δx = this.position.x - collider.position.x; 
        let Δy = this.position.y - collider.position.y;
        let distance = Math.sqrt(Δx * Δx + Δy * Δy);
        if (distance < this.radius + collider.radius) {
            return true;
        }
        return false;
    }
    narrowCollision(collider) { 
        // Checks Collision for both collider
        if (!this.narrowCollisionCalulation(this, collider)) {
            return false;
        }
        // Swap the colliders
        return this.narrowCollisionCalulation(collider, this);
    }

    narrowCollisionCalulation(col1, col2) { 
        for (let i = 0; i < col1.vertices.length; i++) {
            // This will wrap the vertices (0 - 1, 1 - 2, 2 - 0)
            let j = (i + 1) % col1.vertices.length;
            // Calculates the change in y and the change in x
            let axisProj = {
                Δx: -(col1.vertices[j].y - col1.vertices[i].y), // invert the Δy because of the coordinate system
                Δy: col1.vertices[j].x - col1.vertices[i].x
            }

            // Find the end points for this edge
            let thisMin = Infinity;
            let thisMax = -Infinity;
            for (let v = 0; v < col1.vertices.length; v++) {
                let dotProduct = (col1.vertices[v].x * axisProj.Δx + col1.vertices[v].y * axisProj.Δy);
                thisMin = Math.min(thisMin, dotProduct);
                thisMax = Math.max(thisMax, dotProduct);
            }

            let otherMin  = Infinity;
            let otherMax = -Infinity;
            for (let v = 0; v < col2.vertices.length; v++) {
                let dotProduct = (col2.vertices[v].x * axisProj.Δx + col2.vertices[v].y * axisProj.Δy);
                otherMin = Math.min(otherMin, dotProduct);
                otherMax = Math.max(otherMax, dotProduct);
            }
            if (!(otherMax >= thisMin && thisMax >= otherMin)) {
                return false;
            }
        }
        return true;
    }
    render(ctx) {
        ctx.save();

        ctx.beginPath();
        ctx.translate(this.position.x, this.position.y);
        ctx.arc(0, 0, this.radius, 0, Math.PI * 2);
        ctx.closePath();

        ctx.strokeStyle = "black";
        ctx.stroke();

        ctx.restore();
    }
}