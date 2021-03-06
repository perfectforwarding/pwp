/**
 * Converts an HSL color value to RGB. Conversion formula
 * adapted from http://en.wikipedia.org/wiki/HSL_color_space.
 * Assumes h, s, and l are contained in the set [0, 1] and
 * returns r, g, and b in the set [0, 255].
 *
 * @param   Number  h       The hue
 * @param   Number  s       The saturation
 * @param   Number  l       The lightness
 * @return  Array           The RGB representation
 */
function hslToRgb(h, s, l){
    var r, g, b;

    if(s == 0){
        r = g = b = l; // achromatic
    }else{
        function hue2rgb(p, q, t){
            if(t < 0) t += 1;
            if(t > 1) t -= 1;
            if(t < 1/6) return p + (q - p) * 6 * t;
            if(t < 1/2) return q;
            if(t < 2/3) return p + (q - p) * (2/3 - t) * 6;
            return p;
        }

        var q = l < 0.5 ? l * (1 + s) : l + s - l * s;
        var p = 2 * l - q;
        r = hue2rgb(p, q, h + 1/3);
        g = hue2rgb(p, q, h);
        b = hue2rgb(p, q, h - 1/3);
    }

    return [r * 255, g * 255, b * 255];
}

/**
 * Converts an RGB color value to HSL. Conversion formula
 * adapted from http://en.wikipedia.org/wiki/HSL_color_space.
 * Assumes r, g, and b are contained in the set [0, 255] and
 * returns h, s, and l in the set [0, 1].
 *
 * @param   Number  r       The red color value
 * @param   Number  g       The green color value
 * @param   Number  b       The blue color value
 * @return  Array           The HSL representation
 */
function rgbToHsl(r, g, b){
    r /= 255, g /= 255, b /= 255;
    var max = Math.max(r, g, b), min = Math.min(r, g, b);
    var h, s, l = (max + min) / 2;

    if(max == min){
        h = s = 0; // achromatic
    }else{
        var d = max - min;
        s = l > 0.5 ? d / (2 - max - min) : d / (max + min);
        switch(max){
            case r: h = (g - b) / d + (g < b ? 6 : 0); break;
            case g: h = (b - r) / d + 2; break;
            case b: h = (r - g) / d + 4; break;
        }
        h /= 6;
    }

    return [h, s, l];
}

/**
 * Provides requestAnimationFrame in a cross browser way.
 */
window.requestAnimFrame = (function() {
  return window.requestAnimationFrame ||
         window.webkitRequestAnimationFrame ||
         window.mozRequestAnimationFrame ||
         window.oRequestAnimationFrame ||
         window.msRequestAnimationFrame ||
         function(/* function FrameRequestCallback */ callback, /* DOMElement Element */ element) {
           window.setTimeout(callback, 1000/60);
         };
})();


function degToRad(deg)
{
    return deg / 180 * Math.PI;
}

function CreateFullScreenQuad(gl)
{
  this.gl = gl;
  this.vb = gl.createBuffer();

  // Start to fill the buffer
  gl.bindBuffer(gl.ARRAY_BUFFER, this.vb);

  // The quad vb is 2d position + texture coords
  gl.bufferData(gl.ARRAY_BUFFER,
    new Float32Array([
      -1.0, -1.0,       0.0, 0.0,
      1.0, -1.0,        1.0, 0.0,
      -1.0, 1.0,        0.0, 1.0,
      -1.0, 1.0,        0.0, 1.0,
      1.0, -1.0,        1.0, 0.0,
      1.0, 1.0,       1.0, 1.0 ]),    
    gl.STATIC_DRAW);

  this.numComponents = 2;
  this.componentType = gl.FLOAT;
  this.normalize = false;
  this.stride = 4 * 4;    // 4 float elements

  this.posStart = 0;
  this.tcStart = 4;

  this.apply = function(posAttribute, tcAttribute)
  {
    gl.bindBuffer(gl.ARRAY_BUFFER, this.vb);
    if(posAttribute != undefined)
    {      
      gl.vertexAttribPointer(posAttribute, 2, gl.FLOAT, false, 4 * 4, 0);      
      gl.enableVertexAttribArray(posAttribute);                 
    }
    if(tcAttribute  != undefined)
    {
		gl.vertexAttribPointer(tcAttribute, 2, gl.FLOAT, false, 4 * 4, 2 * 4);  
		gl.enableVertexAttribArray(tcAttribute);      
    }
  }
}

function initRenderTarget(gl, pixels_width, pixels_height)
{
	var renderBuffer = {};
	renderBuffer.width = pixels_width;
	renderBuffer.height = pixels_height;
	// Create frame buffer				
	renderBuffer.frameBuffer = gl.createFramebuffer();
	// Set as active frame buffer
	gl.bindFramebuffer(gl.FRAMEBUFFER, renderBuffer.frameBuffer);

	// Create RGBA texture
	renderBuffer.texture = gl.createTexture();
	// Set as active texture
	gl.bindTexture(gl.TEXTURE_2D, renderBuffer.texture);
	// Set the params of the binded texture
	gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, gl.NEAREST);
	gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, gl.NEAREST);
	gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, gl.CLAMP_TO_EDGE);
	gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, gl.CLAMP_TO_EDGE);

	// The texture is going to be used as render target, so initialize with the same size of the framebuffer
	gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, renderBuffer.width, renderBuffer.height, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);

	// Create depth buffer
	renderBuffer.depthBuffer = gl.createRenderbuffer();
	// Set as active render buffer
	gl.bindRenderbuffer(gl.RENDERBUFFER, renderBuffer.depthBuffer);
	gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH_COMPONENT16, renderBuffer.width, renderBuffer.height);

	// Bind the texture to the frame buffer, to be used as color channel
	gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, renderBuffer.texture, 0);
	// Bind the depth buffer to the framebuffer
	gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.RENDERBUFFER, renderBuffer.depthBuffer);

	// Unbind
	gl.bindTexture(gl.TEXTURE_2D, null);
	gl.bindRenderbuffer(gl.RENDERBUFFER, null);
	gl.bindFramebuffer(gl.FRAMEBUFFER, null);		

	//console.log("Render target " + pixels_width + "x" + pixels_height + " created");

	return renderBuffer;		
}

function initRenderTargetTexture(gl, texture)
{
	var renderBuffer = {};
	renderBuffer.width = texture.image.width;
	renderBuffer.height = texture.image.height;
	renderBuffer.texture = texture;

	//gl.bindTexture(gl.TEXTURE_2D, texture);
	//gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, renderBuffer.width, renderBuffer.height, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);

	renderBuffer.frameBuffer = gl.createFramebuffer();
	gl.bindFramebuffer(gl.FRAMEBUFFER, renderBuffer.frameBuffer);

	// Attach a texture 
    gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, texture, 0);

	gl.bindFramebuffer(gl.FRAMEBUFFER, null);		
	//console.log("Render target " + pixels_width + "x" + pixels_height + " created");    

	return renderBuffer;
}

function exists_url_part(partid)
{
	var url = window.location.href;
	var qparts = url.split("?");
	if(qparts.length > 1) {
		for(var i = 1; i < qparts.length; ++i)
		{
			if(qparts[i] === partid) {
				return  true;
			}
		}
	}
}

function loadFile(url, data, callback, errorCallback) 
{
    // Set up an asynchronous request
    var request = new XMLHttpRequest();
    request.open('GET', url, true);

    // Hook the event that gets called as the request progresses
    request.onreadystatechange = function () {
        // If the request is "DONE" (completed or failed)
        if (request.readyState == 4) {
            // If we got HTTP status 200 (OK)
            if (request.status == 200) {
                callback(request.responseText, data)
            } else { // Failed
                errorCallback(url);
            }
        }
    };

    request.send(null);    
}

function loadFiles(urls, callback, errorCallback) 
{
    var numUrls = urls.length;
    var numComplete = 0;
    var result = [];

    // Callback for a single file
    function partialCallback(text, urlIndex) 
    {
        result[urlIndex] = text;
        numComplete++;

        // When all files have downloaded
        if (numComplete == numUrls) {
            callback(result);
        }
    }

    for (var i = 0; i < numUrls; i++) {
        loadFile(urls[i], i, partialCallback, errorCallback);
    }
}

function clamp(val, min, max)
{
	if(val < min)
	{
		val = min;
	}
	if(val > max)
	{
		val = max;
	}

	return val;
}