
//widget plotter functions
function plotOperationRuntime(element, dat) {
    let plot = document.getElementById(element);
    var trace1 = {
            x: dat.x,
            y: dat.y,
            text:dat.id,
            hovertemplate:'<b>%{text}</b>', //ref : https://plotly.com/javascript/hover-text-and-formatting/
            name: 'Line-1',
            type: 'scatter',
            orientation:"h",
            mode: 'markers',
            marker: {
                size:16,
                symbol: 'square',
                color: dat.c
            }
        };
    var data= [trace1];
    var layout = {tracegroupgap :50,showlegend:false, fontsize:18, title: 'Line live status',
                  yaxis:{autorange:'reversed'},
                  margin: {
                    b:20,
                    r:0,
                }
                };
    var config = {displaylogo: false, displayModeBar: false, responsive: true};
    Plotly.newPlot(plot, data, layout,config);

    OnClickDataPoint(plot);
    //goToPageThroughLabel(plot);
}

function plotPieChart(element,val){
    let plot = document.getElementById(element);
    var data = [{
        values: val,
        labels: ['Good', 'Ok', 'Not-Ok'],
        type: 'pie',
        textinfo: "label+percent"
      }];

    var layout = {
    //height: 400,
    //width: 500,
    showlegend:false, 
    fontsize:18, 
    title: {
        text: 'Overal Quality Status',
        y:0.01,
        font : {
            color:'lightblack',
            size: 10,
        },
        pad:{
            b:0,
            l:0,
            r:0,
            t:0
        }
    },
    margin: {
        autoexpand:true,
        b:12,
        l:0,
        pad:0,
        r:0,
        t:15
    }
    };
    var config = {displaylogo: false, displayModeBar: false, responsive: true};
    Plotly.newPlot(plot , data, layout, config);
}

function OnClickDataPoint (plotObject){
    plotObject.on('plotly_click', function( data){
        for(let i =0; i<data.points.length;i++){
            redirectToPage("/PartDetail",params={partId:data.points[i].text});
            console.log(data.points[i].text);
        }
    });
}

function redirectToPage(page, params={}){
    window.location=addParamstoURL(params,page);
    //window.location.href=page;
    //window.location
}

function addParamstoURL(params, url){
    url=url+"?";
    Object.keys(params).forEach((key,index,arr) =>{
        if(index<arr.length-1) {
            url=url+String(key)+"="+String(params[key])+"&";
        }else {
            url=url+String(key)+"="+String(params[key]);
        }
    });
    return url;
}

function goToPageThroughLabel(plotObject){
    plotObject.on('plotly_afterplot', ()=> {
        Plotly.d3.selectAll(".yaxislayer-above").selectAll('text')
          .on("click", function(d) {
            alert("Hello, I am " + d.x);
          });
    } );
}


//redraw feature plotly : 
    //https://community.plotly.com/t/what-is-the-most-performant-way-to-update-a-graph-with-new-data/639
    // https://stackoverflow.com/questions/32116368/plotly-update-data
    //https://plotly.com/javascript/streaming/
// menu feature/buttons in plotly
    //https://codepen.io/plotly/pen/BqYMqq

// function reference in plotly :
    //https://plotly.com/javascript/plotlyjs-function-reference/#plotly-redraw