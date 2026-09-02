// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Fill.asm

// Runs an infinite loop that listens to the keyboard input.
// When a key is pressed (any key), the program blackens the screen,
// i.e. writes "black" in every pixel;
// the screen should remain fully black as long as the key is pressed.
// When no key is pressed, the program clears the screen, i.e. writes
// "white" in every pixel;
// the screen should remain fully clear as long as no key is pressed.

// Put your code here.
    @16384 // Address of the screen memory-mapped register (start of the screen)
    D=A
    @pos
    M=D // Store the address of the first pixel in the variable 'pos'

(LOOP)
    // Capture the keyboard input
    @24576 // Address of the keyboard memory-mapped register
    D=M // Load the keyboard input into D

    @CLEAR
    D;JEQ // If no key is pressed, jump to CLEAR

    @WRITE
    0;JMP // If a key is pressed, jump to WRITE

(CLEAR)
    @pos
    A=M // Load the address of the current pixel into A
    M=0 // Clear the screen (write "white" to the pixel) to the address of A

    @pos
    M=M+1 // Move to the next pixel

    @LOOP
    0;JMP

(WRITE)
    @pos
    A=M // Load the address of the current pixel into A
    M=-1 // Blacken the screen (write "black" to the pixel) to the address of A

    @pos
    M=M+1 // Move to the next pixel

    @LOOP
    0;JMP
