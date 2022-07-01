

kernel void Paint(global uchar* pixels, global uint* iterations, uint iterationCap, uchar mainR, uchar mainG, uchar mainB, uchar inSetR, uchar inSetG, uchar inSetB) {
    int pixNum = get_global_id(0);

    int pixelIterations = iterations[pixNum / 4];  // Which iteration are we looking at
    int subPixel = pixNum % 4;  // Which of hte 4 bytes are we on
    // BGRA

    double iterationRatio = (float)pixelIterations / (float)iterationCap;



    if (subPixel == 3) {  // If I was clever I would design this so it is completely skipped 
        pixels[pixNum] = 255;  // Alpha
    }
    else if (iterationCap == pixelIterations) {  // In set
        if (subPixel == 0) { // B
            pixels[pixNum] = inSetB;
        }
        else if (subPixel == 1) { //G
            pixels[pixNum] = inSetG;
        }
        else if (subPixel == 2) {  // R
            pixels[pixNum] = inSetR;
        }
    }
    else {
        if (subPixel == 0) { // B
            pixels[pixNum] = (int)(iterationRatio * mainB);
        }
        else if (subPixel == 1) { //G
            pixels[pixNum] = (int)(iterationRatio * mainG);
        }
        else if (subPixel == 2) {  // R
            pixels[pixNum] = (int)(iterationRatio * mainR);
        }
    }

}