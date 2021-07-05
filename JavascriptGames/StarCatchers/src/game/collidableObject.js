export class CollidableObject {
    constructor(x, y, radius) {
        this.x = x;
        this.y = y;
        this.radius = radius;
    }
    update1(x, y, vertices) {
        this.x = x;
        this.y = y;
        this.vertices = vertices; // Make a deep copy
    }
    didCollide(object) {
        if (this.broadCollision(object)) {
            return this.narrowCollision(object);
        }
        return false;
    }
    broadCollision(object) {
        let dx = this.x - object.x;
        let dy = this.y - object.y;
        let distance = Math.sqrt(dx * dx + dy * dy);
        if (distance < this.radius + object.radius) {
            return true;
        }
        return false;
    }
    narrowCollision(object) { 
        // for both shapes
        if (!this.narrowCollisionCalulation(this, object)) {
            return false;
        }
        return this.narrowCollisionCalulation(object, this);

    }

    narrowCollisionCalulation(obj1, obj2) { 
        for (let i = 0; i < obj1.vertices.length; i++) {
            // This will wrap the vertices (0 - 1, 1 - 2, 2 - 0)
            let j = (i + 1) % obj1.vertices.length;
            // Calculates the change in y and the change in x
            let axisProj = {
                Δx: -(obj1.vertices[j].y - obj1.vertices[i].y), // invert the Δy because of the coordinate system
                Δy: obj1.vertices[j].x - obj1.vertices[i].x
            }
            // Find the end points for this edge
            let thisMin = Infinity;
            let thisMax = -Infinity;
            for (let v = 0; v < obj1.vertices.length; v++) {
                let dotProduct = (obj1.vertices[v].x * axisProj.Δx + obj1.vertices[v].y * axisProj.Δy);
                thisMin = Math.min(thisMin, dotProduct);
                thisMax = Math.max(thisMax, dotProduct);
            }

            let otherMin  = Infinity;
            let otherMax = -Infinity;
            for (let v = 0; v < obj2.vertices.length; v++) {
                let dotProduct = (obj2.vertices[v].x * axisProj.Δx + obj2.vertices[v].y * axisProj.Δy);
                otherMin = Math.min(otherMin, dotProduct);
                otherMax = Math.max(otherMax, dotProduct);
            }
            if (!(otherMax >= thisMin && thisMax >= otherMin)) {
                return false;
            }
        }
        return true;
    }
    renderCollider(ctx) {
        ctx.save();

        // Show broad collider
        ctx.beginPath()
        ctx.arc(this.x, this.y, this.radius, 0, Math.PI * 2);
        ctx.closePath();
        ctx.strokeStyle = "white";
        ctx.stroke();
        
        // Show narrow collider
        ctx.beginPath()
        this.vertices.forEach(vertex => {
            ctx.lineTo(vertex.x, vertex.y);
        });
        ctx.closePath();
        ctx.strokeStyle = "white";
        ctx.stroke();

        ctx.restore();
    }
}