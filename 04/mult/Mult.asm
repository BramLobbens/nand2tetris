// This file is part of www.nand2tetris.org
// and the book "The Elements of Computing Systems"
// by Nisan and Schocken, MIT Press.
// File name: projects/04/Mult.asm

// Multiplies R0 and R1 and stores the result in R2.
// (R0, R1, R2 refer to RAM[0], RAM[1], and RAM[2], respectively.)
//
// This program only needs to handle arguments that satisfy
// R0 >= 0, R1 >= 0, and R0*R1 < 32768.
    @i // Address of the loop counter 'i'
    M=0 // Initialize i to 0
    @1
    D=M // Load R1 into D
    @i
    M=D // Initialize i with the value of R1

(LOOP)
    @0
    D=M // Load R0 into D

    @2
    M=D+M // Add R0 to R2

    @i
    D=M-1 // Decrement i
    @END
    D;JEQ // If i is 0, jump to END

    @i
    M=D

    @LOOP
    0;JMP // Repeat the loop

    @END
    D;JEQ
(END)
    @END
    0;JMP