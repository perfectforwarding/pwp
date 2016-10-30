

var globals = {};

function getDiv(i)
{
    var pdiv = document.getElementById("shaderPreview" + (i));

    return pdiv;
}

function createPreview(base, id)
{
	base.onclick = function() { console.log("click-base"); };

	var url = base.getAttribute('data-url');

	var sizeX = base.offsetWidth;
	var sizeY = base.offsetHeight;

	//-- Create preview object
	var preview = document.createElement("object");
	
	preview.style.width = sizeX + "px";
	preview.style.height = sizeY + "px";

	preview.type = "text/html";
	preview.data = "public/" + url + "?preview";;

	base.appendChild(preview);

	//-- Create div to click
	var adiv = document.createElement('div');
	adiv.style.position = "absolute";

	adiv.style.top = preview.offsetTop;
	adiv.style.width = sizeX + "px";
	adiv.style.height = sizeY + "px";	
	//adiv.style.backgroundColor = 'green';
/*
	adiv.onclick = function() { 
		console.log("click-base"); 
		window.location.href = "public/" + url;		
	};	
*/
	base.appendChild(adiv);	

	//-- Create a-href elemnt
	var a = document.createElement('a');
	a.href = "public/" + url;
	//a.innerHTML = "Link";

	a.style.position = "absolute";
	a.style.top = preview.offsetTop;
	a.style.width = sizeX + "px";
	a.style.height = sizeY + "px";	

	adiv.appendChild(a);	
}

function pageInit()
{
	console.log("pageInit");

	// Get number of widgets
	var widgets = document.getElementById('widgets');
	var nwidgets = parseInt(widgets.getAttribute('data-number'));
	console.log("Found " + (nwidgets + 1) + " widgets");

	// Calc widgets width
	var docWidth = document.body.clientWidth;
	var widgetWidth = docWidth / (nwidgets + 2);
	var widgetHeight = widgetWidth * 10 / 16;

	console.log("docWidth: " + docWidth);
	console.log("widgetWidth: " + widgetWidth);

	var xpos = widgetWidth;				// First widget position
	// Iterate through all widgets
    for(var i = 0; i < nwidgets; i++)
    {
		var pdiv = getDiv(i);
		if(pdiv)
		{
			//console.log(xpos);

			// Set widget position
			pdiv.style.position = "absolute";
			pdiv.style.left = xpos + "px";

			// Set widget size
			pdiv.style.width = widgetWidth + "px";
			pdiv.style.height = widgetHeight + "px";

	        var xres = pdiv.offsetWidth;
	        var yres = pdiv.offsetHeight;

	        // Create widget view
	        var pv = createPreview(pdiv, i);

	        // Next widget position
	        xpos += widgetWidth  * 1;
	    }
	}

	initGL();	
	initFullScreenQuad();
	initShaders();
}

function pageResize()
{

}

//--- GL stuff

function initGL()
{
	// Init canvas
	globals.canvas = document.getElementById("canvas");
	globals.canvas.width = window.innerWidth;
	globals.canvas.height = window.innerHeight;
	globals.canvas.ctx = globals.canvas.getContext('webgl');


	if(!globals.canvas.ctx)
	{
		throw "Failed initializing WebGL";
	}			
}

function initFullScreenQuad()
{
	globals.fullScreenQuad = new CreateFullScreenQuad(globals.canvas.ctx);
}

function initShaders()
{
	var gl = globals.canvas.ctx;

	loadShaderProgram(
		gl, 
		'background.vs', 
		'background.fs',
		function (shaderProgram) {
			shaderProgram.aVertexPosition = gl.getAttribLocation(shaderProgram, "aVertexPosition");
			shaderProgram.uResolution = gl.getUniformLocation(shaderProgram, "uResolution");     

	        globals.shader = shaderProgram;

	        drawBackground();
		},
			function (msg) {
	    	alert(msg);
		}
	);				
}

function drawBackground()
{
//	console.log("hello"); 

	var gl = globals.canvas.ctx;

	// Disable depth test
	gl.disable(gl.DEPTH_TEST); 		    	
	// Setup viewport
	gl.viewport(0, 0, globals.canvas.width, globals.canvas.height);
	//console.log("Viewport " + globals.canvas.width + "x" + globals.canvas.height);
	// Clear render target
	gl.clearColor(0.0, 0.0, 0.0, 1.0);
	gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);    		    	

	// Set shader program
	gl.useProgram(globals.shader);

	// Set attributes
	globals.fullScreenQuad.apply(globals.shader.aVertexPosition); 	

	// Set uniforms
	gl.uniform2f(globals.shader.uResolution, globals.canvas.width, globals.canvas.height);

	// Draw the quad
	gl.drawArrays(gl.TRIANGLES, 0, 6);

	//requestAnimFrame(drawBackground);		    	
}