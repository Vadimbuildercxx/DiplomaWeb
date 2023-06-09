﻿class Vector2 {
    constructor(x, y) {
        this.x = x;
        this.y = y;
    }
    dot(other) {
        return this.x * other.x + this.y * other.y;
    }
}

function Shuffle(tab) {
    for (let e = tab.length - 1; e > 0; e--) {
        let index = Math.round(Math.random() * (e - 1)),
            temp = tab[e];

        tab[e] = tab[index];
        tab[index] = temp;
    }
}

function MakePermutation() {
    let P = [];
    for (let i = 0; i < 256; i++) {
        P.push(i);
    }
    Shuffle(P);
    for (let i = 0; i < 256; i++) {
        P.push(P[i]);
    }

    return P;
}
let P = MakePermutation();

function GetConstantVector(v) {
    //v is the value from the permutation table
    let h = v & 3;
    if (h == 0)
        return new Vector2(1.0, 1.0);
    else if (h == 1)
        return new Vector2(-1.0, 1.0);
    else if (h == 2)
        return new Vector2(-1.0, -1.0);
    else
        return new Vector2(1.0, -1.0);
}

function Fade(t) {
    return ((6 * t - 15) * t + 10) * t * t * t;
}

function Lerp(t, a1, a2) {
    return a1 + t * (a2 - a1);
}

function Noise2D(x, y) {
    let X = Math.floor(x) & 255;
    let Y = Math.floor(y) & 255;

    let xf = x - Math.floor(x);
    let yf = y - Math.floor(y);

    let topRight = new Vector2(xf - 1.0, yf - 1.0);
    let topLeft = new Vector2(xf, yf - 1.0);
    let bottomRight = new Vector2(xf - 1.0, yf);
    let bottomLeft = new Vector2(xf, yf);

    //Select a value in the array for each of the 4 corners
    let valueTopRight = P[P[X + 1] + Y + 1];
    let valueTopLeft = P[P[X] + Y + 1];
    let valueBottomRight = P[P[X + 1] + Y];
    let valueBottomLeft = P[P[X] + Y];

    let dotTopRight = topRight.dot(GetConstantVector(valueTopRight));
    let dotTopLeft = topLeft.dot(GetConstantVector(valueTopLeft));
    let dotBottomRight = bottomRight.dot(GetConstantVector(valueBottomRight));
    let dotBottomLeft = bottomLeft.dot(GetConstantVector(valueBottomLeft));

    let u = Fade(xf);
    let v = Fade(yf);

    return Lerp(u,
        Lerp(v, dotBottomLeft, dotTopLeft),
        Lerp(v, dotBottomRight, dotTopRight)
    );

}

function genMap(width, height, isAngle, scale) {
    pixels = []
    for (let y = 0; y < height; y++){
        for (let x = 0; x < width; x++){
            let n = Noise2D(x * 0.01 * scale, y * 0.01 * scale);
            n += 1.0;
            n *= 0.5;
            let rgb = Math.round(255*n);
            //ctx.fillStyle = "rgba("+rgb+","+rgb+","+rgb+",1.0)";
            //ctx.fillRect(x, y, 1, 1);
            if (isAngle) {
                pixels.push(rgb / 255 * Math.PI* 2);
            } else {
                pixels.push(rgb);
            }
            
        }
    }
    return pixels;
}
//const ctx1 = document.getElementById("canvas2").getContext("2d");
//ctx1.fillStyle = "#343635";
//Generates a black/white heightmap
//for(let y = 0; y < 500; y++){
//    for(let x = 0; x < 500; x++){
//        let n = Noise2D(x*0.01, y*0.01);
//        n += 1.0;
//        n *= 0.5;
//        let rgb = Math.round(255*n);
//        ctx.fillStyle = "rgba("+rgb+","+rgb+","+rgb+",1.0)";
//        ctx.fillRect(x, y, 1, 1);
//    }
//}

//Generates a color heightmap with fractal brownian motion

//for (let y = 0; y < 500; y++) {
//    for (let x = 0; x < 700; x++) {
//        let n = 0.0,
//            a = 1.0,
//            f = 0.005;
//        for (let o = 0; o < 8; o++) {
//            let v = a * Noise2D(x * f, y * f);
//            n += v;

//            a *= 0.5;
//            f *= 2.0;
//        }

//        n += 1.0;
//        n *= 0.5;
//        let rgb = Math.round(255 * n);
//        if (n < 0.5)
//            ctx1.fillStyle = "rgba(0.0,0.0," + (rgb * 2) + ",1.0)";
//        else if (n < 0.9)
//            ctx1.fillStyle = "rgba(0.0," + rgb + "," + Math.round(rgb * 0.5) + ",1.0)";
//        else
//            ctx1.fillStyle = "rgba(" + rgb + "," + rgb + "," + rgb + ",1.0)";
//        ctx1.fillRect(x, y, 1, 1);
//    }
//}