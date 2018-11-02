
/* SCRIPT FOR ILLUSTRATOR
 * Save all artboards to a selected folder in scales needed by the Universal Windows Platform Apps.
 * 
 * By: @LeisvanCordero
 * 
*/


var folder = Folder.selectDialog();
var document = app.activeDocument;

if (document && folder) {

    saveAllArtboardsToScale(100);
    saveAllArtboardsToScale(125);
    saveAllArtboardsToScale(150);
    saveAllArtboardsToScale(200);
    saveAllArtboardsToScale(400);
}

function saveAllArtboardsToScale(scale) {
	var i, artboard, file, options;
	
	for (i = document.artboards.length - 1; i >= 0; i--) {
		document.artboards.setActiveArtboardIndex(i);
		artboard = document.artboards[i];


		var fileName = artboard.name + ".scale-" + scale;
		file = new File(folder.fsName + "/" + fileName + ".png");
		
		options = new ExportOptionsPNG24();
		options.verticalScale = scale;
		options.horizontalScale = scale;
		options.artBoardClipping = true;
		options.antiAliasing = true;
		options.transparency = true;
		
		document.exportFile(file, ExportType.PNG24, options);
	}
}
