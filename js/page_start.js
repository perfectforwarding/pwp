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

	//return;

	// Get number of widgets
	var wisgets = document.getElementById('widgets');
	var nwidgets = widgets.getAttribute('data-number');
	console.log("Found " + nwidgets + " widgets");

	// Calc widgets width
	var docWidth = document.body.clientWidth;
	var widgetWidth = docWidth / (nwidgets * 2 + 1);
	var widgetHeight = widgetWidth / 2;

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
	        xpos += widgetWidth  * 2;
	    }
	}

}

function pageResize()
{

}