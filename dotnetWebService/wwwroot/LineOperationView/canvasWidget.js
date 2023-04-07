const IMG_SRC_PATH="/img/station.png";
const background_Image = '/img/floor.jpg';


function buildLayout(imagePath,config,machine_status) {
    // to structure all the animation assets
    //front canvas
    const canvas = document.getElementById('builtLayout');
    canvas.width=config.canvasWidth;
    canvas.height=config.canvasHeight;
    const ctx = canvas.getContext('2d');
    attachEventListener(canvas);
    //
    //background canvas
    const backFloor = document.getElementById('backgroundFloor');
    backFloor.width=config.canvasWidth;
    backFloor.height=config.canvasHeight;
    const backgroundCtx= backFloor.getContext('2d');
    //
    //asset sizing in canvas;
    let decreaseFactor = 5;
    let size = Math.min(Math.floor(config.canvasWidth/decreaseFactor),Math.floor(config.canvasHeight/decreaseFactor));
    config.imageDisplayWidth= size;
    config.imageDisplayHeight = size; 
    //
    //constructing the front structure 
    let layoutSize=new Array(config.station_layout.length,4); // [a,...b] a:total no. of items, B: index at which row starts 
    config.layoutSize=layoutSize;
    const imageArray= imageCarouselGenerator(layoutSize[0],imagePath);
    //
    //initiating the complete rendering
    let image = imageArray[0];
    image.onload= ()=>{
        animate(ctx,imageArray, config=config, machine_status);
        loadBackground(backgroundCtx,background_Image,config);
    };
}

let animate = async function(ctx,imageArray, config, machine_status) {    
    //setup of structure of the machine layout   
    let imageArray1=imageArray.slice(0,config.layoutSize[0]);
    let imageArray2=imageArray.slice(config.layoutSize[0]);
    //
    //machine size local variable;
    let imageDisplayHeight=config.imageDisplayHeight;
    let imageDisplayWidth=config.imageDisplayWidth;
    //
    //setting the arrowsize & Board size
    let arrowSize=50;
    let statusBoardSize=Math.floor(Math.max(imageDisplayHeight,imageDisplayWidth)/1.4);
    //
    //initiation of plotting variables
    let startXoffset = Math.floor(config.canvasWidth*0.05);
    let startYoffset = Math.floor(Math.max(statusBoardSize+config.imageDisplayHeight,config.canvasHeight*0.05));
    let originX = 0 + startXoffset;
    let originY = 0+ startYoffset;//config.canvasHeight-imageDisplayHeight;
    let dX= Math.floor((config.canvasWidth-startXoffset)/config.layoutSize[0]);
    //not yet utilized the following value
    let dividers = config.layoutSize.slice(1);
    let dY = Math.floor(config.canvasHeight/2); //Math.floor(dX*Math.tan((30/180)*Math.PI));
    //
    //plotting
    imageArray1.forEach( (element,index,array) => {
        ctx.drawImage(element, originX + index*dX , originY, 
                        imageDisplayWidth, imageDisplayHeight);
        if(array.length>index+1) {
        ctx.drawImage(getArrowImage(), 0, Math.floor(1*600/3), 
                        Math.floor(1*600/3), Math.floor(1*600/3),     
                        originX + index*dX + imageDisplayWidth, originY,
                      arrowSize,arrowSize );
        }
        //ctx.save();
        getProcessName(ctx,config.station_layout[index],
            statusBoardSize,
            originX + index*dX + Math.floor(imageDisplayWidth*0.3), originY -20, 
            machine_status[index]);
        //ctx.restore();     
    });
    //
    //console.log("animation run");
}

function getArrowImage( imagePath= '/img/newArrow.png'){
    const image= new Image();
    image.src=imagePath;
    return image;
}

function colourShadeFromStatus(status){
    if (status=='ok'){
        return 'lightgreen';
    }
    if (status=='not-ok'){
        return 'red';
    }
    if (status=='not-init'){
        return "#38f";
    }
    return "#38f";
}

function getProcessName(ctx, val, displaySize,posX, posY, status){
    ctx.fillStyle = colourShadeFromStatus(status);
    const font_size=Math.floor(displaySize/3);
    const pad= 5;
    ctx.fillRect(posX-pad, posY+pad, font_size + displaySize, -font_size-2*pad);
    ctx.fillStyle= "black";
    ctx.font = `${font_size}px Arial`;
    ctx.fillText(val, posX, posY);
    ctx.strokeStyle = "#38f";
    ctx.strokeRect(posX-pad, posY+pad, font_size + displaySize, -font_size-2*pad);
}

function imageCarouselGenerator(number, src) {
    const result= new Array();
    for(i=0;i<number;i++){
        const image = new Image();
        image.src = src;
        result.push(image)
    }
    return result;
}

async function loadBackground(ctx,imagePath,config){
    const image = new Image();
    image.src=imagePath;
    //need to find better background
    //ctx.drawImage(image,0,0, config.canvasWidth, config.canvasHeight);
}

async function attachEventListener (canvasElement){
    canvasElement.addEventListener ("click", ()=>{
        window.location="/OperationCharacteristicStats?operation=op10";
    } );
    return true;
}

