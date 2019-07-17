# Logo
An implementation of the educational programming language [Logo](https://en.wikipedia.org/wiki/Logo_(programming_language)) with the addition of variables, evaluations and basic control functions (if/then/else/loops etc.)

![Screenshot](https://github.com/James-P-D/Logo/blob/master/screenshot.gif)

## Commands

Below is a quick overview of the available commands. For some sample programs, see the [examples](https://github.com/James-P-D/Logo/tree/master/src/Logo/Logo/examples) folder.

### Basic Movements

The turtle can be moved in four directions without turning:
```
forward X;
backward X;
left X;
right X;
```

In each case `X` can be an integer literal, e.g. `forward 100;`, or can be a variable, e.g. `forward length;` (where `length` has been declared as a `number` variable), or it can be an evaluation consisting of integer literals and/or variables, e.g. `forward 2 * length;`.

It is also possible to rotate the turtle clockwise or anti-clockwise:
```
rightturn X;
leftturn X;
```

Again, `X` can be an integer literal, a variable or an evaluation consisting of integers and variables.

It is also possible to manually set the `X` and `Y` positions, or the direction of the turtle:
```
setx X;
sety Y;
setdirection D;
```

As usual, the parameter can be a integer literal, variable or evaluation. When setting the direction, the value `D` will wrap around `0` and `360`.

If you wish to simply centre the turtle and set the direction to upward (zero) we can use the following command:
```
centerturtle;
```

We can toggle the displaying of the turtle with the following commands:
```
hideturtle;
showturtle;
```

### Colours and Pen Up/Down

By default the turtle pen colour will always be black, but it is possible to set the ARGB components:
```
colora X;
colorr X;
colorg X;
colorb X;
```

`X` can be an integer literal, a variable or an evaluation consisting of integers and variables, but the value will be capped to the range `0-255`.

By default the turtle will always draw lines as it moves across the canvas. If you wish to toggle whether the turtle pen is touching the canvas or not, you will need the following commands:
```
penup;
pendown;
```

### Variables

Variables can be declared using the keyword `number` followed by a variable name. 
```
number length;
```

Once variables have been created, we can assign values to them with the assignment operator `=`:
```
length = 10;
```

It is also possible to initialise variables:
```
number width = 15.8;
```

We can also initialise variables with evaluations:
```
number area = length * width;
```

Finally it is possible to create `boolean` variables:
```
boolean bool1;
boolean bool2 = false;
boolean bool3 = bool1 && bool2;
```

### Getting Values

A small number of commands return values. These commands are as follows:
```
<nummber> getx;
<nummber> gety;
<nummber> getdirection;
<nummber> getcolora;
<nummber> getcolorr;
<nummber> getcolorg;
<nummber> getcolorb;
<boolean> ispenup;
<boolean> ispendown;
```

So, for example, if we wished to save the X and Y values of the turtle before moving it, we might use:
```
number savedX = getx;
number savedY = gety;
```

### Repeat Loops

We can repeat a set of commands by using the `repeat` command:
```
repeat N {
  Command1;
  Command2;
  ..
  CommandN;
}
```

Where `N` can be an integer literal, variable or evaluation.

For example, to draw a square we might use the following:
```
repeat 4 {
  forward 100;
  rightturn 90;
}
```

### If..Then..Else, Break and Continue

The interpeter also supports `if..then..else` commands, plus the ability to `break` from loops or to `continue`.

For example, if we wished to draw a semi-circle we could use the following:
```
number counter = 0;
repeat 36 {
  if(counter <= 18) {
    forward 10;
    rightturn 10;
  } else {
    break;
  }
  counter = counter + 1;
}
```

### Do..While Loops

The interpeter also supports `do..while` loops:
```
number counter = 0;
while (counter < 36) {
  
  forward 10;
  rightturn 10;
  
  counter = counter + 1;
}
```

### Miscellaneous Commands

