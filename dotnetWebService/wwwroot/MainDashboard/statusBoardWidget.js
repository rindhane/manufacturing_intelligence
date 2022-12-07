function populateStatusBlock(master_elem,config,valStore){
    master_elem.innerHTML='';
    master_elem.appendChild(statusHeaderBlock(config));
    master_elem.appendChild(statusIndicatorContainer(config,valStore));
    return master_elem;
}

function statusHeaderBlock(config){
    let result = document.createElement('ul');
    result.setAttribute('class','Status-Header');
    let itemTypes= Array("Ok","NOk","Total", "Quality"); // matching the order of config.statusIndicatorBlockTypes
    let temp = itemTypes.map((item_type,index)=> {
        let elem = document.createElement('li');
        elem.innerText=item_type;
        return elem;
    });
    temp.forEach(elem=>{
        result.appendChild(elem);
    });
    return result;
}

function statusIndicatorContainer(config,valStore){
    let result = document.createElement('div');
    result.setAttribute('class','statusIndicatorsContainer');
    valStore.forEach((arr,ops)=>{
        result.appendChild(statusIndicatorBlock(arr,config));
    });
    return result ;
}

function statusIndicatorBlock(valArray,config){
    let result = document.createElement('div');
    result.setAttribute('class','statusIndicators');
    let blockTypes= config.statusIndicatorBlockTypes;
    let temp = blockTypes.map((class_type,index)=> {
        let elem = document.createElement('div');
        elem.setAttribute('class',class_type);
        if (index<3){
            elem.innerText=textFormatter(valArray[index],'regular');
            return elem;
        }
        elem.innerText=textFormatter(valArray[index],'percentage');
        return elem;
    });
    temp.forEach(elem=>{
        result.appendChild(elem);
    });
    return result;
}

function textFormatter(val, type){
    if (type=='regular'){
       return LargeNumberShortner(val);
    }
    if (type=='percentage'){
        return val.toString()+"%";
    }
}

function LargeNumberShortner(val){
    if(val>1000){
        return Math.floor(val/1000).toString() +'ks';
    }
    return val.toString();
}

