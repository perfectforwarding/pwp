<!DOCTYPE html>

<html>

<head>

	<title>Alfonso Comín personal page.</title>

	<link rel="stylesheet" type="text/css" href="./css/style.css"/>

	<script type="text/javascript" src="./js/page_start.js"></script>

<!--
	<style>
		html, body {
			background-color:#000000;
		}	
	</style>
-->


  <!--
  <link rel="stylesheet" type="text/css" href="data/common.css" media="all">
  -->
</head>

<body onload="pageInit()" onresize="pageResize()">

<center>
<h1 >PHP5 & WebGL experiments</h1>
<div style="height: 10vh;" ></div>
</center>


<?php

	$numberOfShaders = 0;

	if($handle = opendir('./public'))
	{
		while(($entry = readdir($handle)) != false)
		{
			if(substr($entry, 0, 1) != ".")
			{
				//echo "<a href=\"public/$entry\">$entry</a><BR/>";

				echo "<a id=\"shaderPreview$numberOfShaders\" class=\"shaderPreview\" data-url=\"$entry\"></a>";
				
				//echo "<object type=\"text/html\" data=\"public/$entry\" width=\"400\" height=\"400\"></object>";
				
				//error_log("<a href=\"$entry\">$entry</a><BR/>");

				++$numberOfShaders;
			}
		}
	}

	echo "<div id='widgets' data-number='$numberOfShaders'></div>";

?>

<!--
<iframe src="http://byethost.com/byet_default_home_page.php" name="Defailt Home Page" scrolling="no" frameborder="no" align="center" height = "900px" width = "600px">
-->
<!--
<iframe src="simple-webgl.html" name="Simple WebGL" scrolling="no" frameborder="no" width="640px" height="480px">
</iframe>
-->


</body>

</html>
