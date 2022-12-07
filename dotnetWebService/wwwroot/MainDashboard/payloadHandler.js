//payload Extractor functions: 
function updateFromPayload(payload, master){
    //payload is a JSON Array
    payload.forEach(element => {
        temp_store = master.get(element.operation);
        temp_store.x.push(getPlotTimeEntry(element.timeStamp));
        temp_store.y.push(element.operation);//master.y.push(getOperationStage(element.operation));
        temp_store.c.push(getColourCode(element.status));
        temp_store.id.push(element.id);
    }); 
    return master;  
}

function calculateValsfromStore(master_store){
    let dat={ //this acts as the input for the operationRuntime Chart
        x:[],
        y:[],
        c:[],
        id:[]
    };
    let valStore= new Map();//this acts as the input for statusBlock Widget
    master_store.forEach((value,key)=>{
        dat.x=dat.x.concat(value.x);
        dat.y=dat.y.concat(value.y);
        dat.c=dat.c.concat(value.c);
        dat.id=dat.id.concat(value.id);
        if(value.c.length > 0) {
            valStore.set(key,getOperationStats(value.c));
        }
    });
    let [okVal,notOkVal,total, qualityVal]=getOperationStats(dat.c); //gives output as [okVal,notOkVal,total, qualityVal]
    let good = Math.floor(okVal*0.1);
    let pieResult=[ //input for the pieChart
                    good, 
                    okVal-good,
                    notOkVal
        ]; // % in order ['Good', 'Ok', 'Not-Ok']
    return [dat,valStore,pieResult];
}

//#region helper functions
function sleep(ms){
    return new Promise(resolve=> setTimeout(resolve,ms));
}

function dateEntryParamsFromTimestamp(num){
    let temp = new Date(num);
    return [temp.getFullYear(), temp.getMonth(),temp.getDate(),
            temp.getHours(),temp.getMinutes(),temp.getSeconds()];
}  
function getDateEntry(year,month,date, hour, minutes, seconds){
    let result = new Date(year,month,date,hour,minutes,seconds);
    let minutes_string = String(result.getMinutes()).padStart(2,'0'); //reference https://stackoverflow.com/questions/1127905/how-can-i-format-an-integer-to-a-specific-length-in-javascript
    let timeString = `${result.getHours()}:${minutes_string}:${result.getSeconds()}`;
    let dateString = `${result.getFullYear()}-${result.getMonth()}-${result.getDate()}`;
    let resultString= dateString+ ' ' + timeString;
    return resultString; 
}

function getPlotTimeEntry(timeStamp){
    let temp = getDateEntry(...dateEntryParamsFromTimestamp(timeStamp));
    return temp;
}

function getOperationStage(index){
    return `OP${index*10}`;
}

function getColourCode(status){
    if (status=='ok') {
        return 'rgba(11, 231, 44,1)';
    }
    if (status=='not-ok'){
        return 'rgba(222,45,38,0.8)';

    }
    return 'rgba(222,45,38,0.8)';
}

function processNameGenerator(num){
    let result = [];
    for (let i=0;i<num;i++){
        result.push(`OP${(i+1)*10}`);
    }
    return result;
    
}

function lastOperationStatusFilter(payload, station_master){
    let result = Array(station_master.length);
    payload.forEach(element=>{
        //converting "OP20" to index 1
        let position = parseInt(element.operation.slice(2))/10 -1 ; 
        result[position]=element.status;
    });
    return result;
}
//
function updateOperationStatusOfConfigMachines(config,current_machine_status){
    current_machine_status.forEach((element,index)=>{
        config.machine_status[index]= element ? element : config.machine_status[index]; 
    })
}
//
function updateOperationStatusOflocalMachines(previous_machine_status,current_machine_status){
    current_machine_status.forEach((element,index)=>{
        previous_machine_status[index]= element ? element : config.machine_status[index]; 
    })
    return previous_machine_status;
}
//
function getOperationStats(colourArray){
    let total = colourArray.length;
    let okVal=colourArray.filter(elem=>{
        return elem==getColourCode('ok');
    }).length;
    let notOkVal=total-okVal;
    let qualityVal=Math.floor(okVal/total*100);
    return [okVal,notOkVal,total, qualityVal] ;//following the sequence of MASTER_CONFIG.statusIndicatorBlockTypes
} 
//#endregion

//#region random data generation in testing
async function runRandomDemo(config=MASTER_CONFIG, stepTime=2000){
    let previous_machine_status=config.machine_status;
    let local_store= config.MASTER_STORE;
    while(true){
        let new_payload=payloadGenerator(num_items=10,config); // num_items is number of random points of data 
        let current_machine_status = lastOperationStatusFilter(new_payload, 
                                                            station_master=config.station_layout);
        previous_machine_status = updateOperationStatusOflocalMachines(previous_machine_status,current_machine_status)
        // previous discarded alternative function updateOperationStatusOfConfigMachines(config,current_machine_status);
        local_store=updateFromPayload(new_payload,local_store);
        let [dat,valStore, pieResult]= calculateValsfromStore(local_store);
        //config.valStore = valStore;
        buildLayout(IMG_SRC_PATH,config,previous_machine_status);
        populateStatusBlock(STATUS_BLOCK_ELEMENT,config,valStore);
        plotPieChart(plotElement2, pieResult);
        plotOperationRuntime(plotElement1,dat);
        await sleep(stepTime);
    }
  }

function okVsNotGenerator(){
    let rand = Math.floor(Math.random() *10 +1);
    if(rand % 10 >8){ // 30% rejection rate
        return 'not-ok';
    }
    return 'ok';
}

function randomPayloadItemGenerator (config){
    let result= {};
    result.id=randomIdGenerator(2,6);
    result.operation= config.station_layout[Math.floor(Math.random()*config.station_layout.length)];
    result.timeStamp=Date.now()-Math.floor(Math.random()*20000);
    result.status= okVsNotGenerator();
    return result;
}

function payloadGenerator(num_items,config){
    let result = [];
    for(let i=0; i<num_items; i++) {
        result.push(randomPayloadItemGenerator(config));
    }
    return result;
}

function randomIdGenerator(idLength,serialLength){
    let string = 'abcdefghijklmnoqprstuvwyzxABCDEFGHIJKLMNOQPRSTUYWVZX';
    let nums = '0123456789';
    let result='';
    for(let i =0; i<idLength; i++){
        result= result+string[Math.floor(Math.random()*52)%52];
    };
    for(let i =0; i<serialLength; i++){
        result= result+nums[Math.floor(Math.random()*10)%10];
    };
    return result;
}
//#endregion

//#region sample data
let DAT_SAMPLE= {
    x:['2013-10-04 22:23:00', '2013-11-06 22:23:00', '2013-12-04 22:23:00', '2013-12-09 22:23:00' ],
    y:['OP10','OP10','OP20','OP30'],
    c:['rgba(11, 231, 44,1)', 'rgba(11, 231, 44,1)', 'rgba(222,45,38,0.8)','rgba(11, 231, 44,1)']
    }
let server_payload_SAMPLE=[
        {
            id:"SNABC",
            operation:1,
            timeStamp:1668351025744,
            status:'ok'
        },
        {
            id:"SNABC",
            operation:2,
            timeStamp:1668351035744,
            status:'ok'
        }
    ];
//#endregion 

