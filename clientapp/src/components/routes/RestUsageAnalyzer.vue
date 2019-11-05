<template>
    <div class="container-fluid">
        <div class="d-sm-flex align-items-center justify-content-between mb-4">
            <h2 class="h3 mb-0 text-gray-800">Rest Usage Analyzer</h2>
            <b-form-select
                class="mb-2 mr-sm-2 mb-sm-0 col-md-4"
                v-model="selSourceId"
                :options="sources"
                text-field="FormattedName"
                value-field="SourceId">
                </b-form-select>
                {{selSourceId}}
        </div>
        <div class="row">
            <div class="col mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <div class="row no-gutters align-items-center">
                            <b-form inline>
                                <label class="mr-sm-2">From</label>
                                <datetime input-class="form-control m-2" v-model="requestModel.FromTime" title="From" type="datetime" format="dd.MM.yyyy hh:mm:ss" ></datetime>
                                <label class="mr-sm-2">To</label>
                                <datetime input-class="form-control m-2" v-model="requestModel.ToTime" title="To" type="datetime" format="dd.MM.yyyy hh:mm:ss"></datetime>
                                <label class="mr-sm-2">Include Path</label>
                                <b-input
                                    class="mb-2 mr-sm-2 mb-sm-0"
                                    v-model="requestModel.ExcludeRest"
                                    placeholder="Include Path"
                                    ></b-input>
                                <label class="mr-sm-2">Exclude Path</label>
                                <b-input
                                    class="mb-2 mr-sm-2 mb-sm-0"
                                    v-model="requestModel.ExcludeRest"
                                    placeholder="Exclude Path"
                                    ></b-input>
                                <b-button @click="requestData()" variant="success" class="float-right"><b-spinner v-if="loading" label="Spinning" small></b-spinner> Request</b-button>
                            </b-form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row" v-if="selEndPoint.endPoint">
            <div class="col-md-12 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <div class="card-header">
                            <h5>Requests over Time</h5>
                        </div>
                        <apexchart type=area height=350 :options="timeChartOptions" :series="timeSeries" />
                    </div>
                </div>
            </div>
        </div>
        <div class="row" v-if="selEndPoint.endPoint">
            <div class="col-md-4 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <table class="table table-striped table-sm">
                            <thead>
                                <tr>
                                    <td>EndPoint</td>
                                    <td>{{selEndPoint.endPoint | max20LastLength }}</td>
                                </tr>
                            </thead>
                            <tbody>
                            <tr>
                                <td>Error Count</td>
                                <td>{{selEndPoint.errorCount}}</td>
                            </tr>
                            <tr>
                                <td>Request Count</td>
                                <td>{{selEndPoint.requestCount}}</td>
                            </tr>
                            <tr>
                                <td>Maximum Request Time</td>
                                <td>{{selEndPoint.maximumRequestTime | ticksToSecond}}</td>
                            </tr>
                            <tr>
                                <td>Average Request Time</td>
                                <td>{{selEndPoint.averageRequestTime | ticksToSecond}}</td>
                            </tr>
                            <tr>
                                <td>Median Request Time</td>
                                <td>{{selEndPoint.medianRequestTime | ticksToSecond}}</td>
                            </tr>
                            <tr>
                                <td>Minimum Request Time</td>
                                <td>{{selEndPoint.minimumRequestTime | ticksToSecond}}</td>
                            </tr>
                            <tr>
                                <td>Absolute Request Time</td>
                                <td>{{selEndPoint.absoluteRequestTime | ticksToSecond}}</td>
                            </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>

            <div class="col-md-4 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-header">
                        <h5>Request Distribution</h5>
                    </div>
                    <div class="card-body">
                        <div id="chart">
                            <apexchart type="donut" height="300" :options="chartOptions" :series="countSeries"/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-header">
                        <h5>Error Distribution</h5>
                    </div>
                    <div class="card-body">
                        <div id="chart">
                            <apexchart type="donut" height="300" :options="chartOptions" :series="errorSeries"/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-header">
                        <h5>Median Request Time Distribution</h5>
                    </div>
                    <div class="card-body">
                        <div id="chart">
                            <apexchart type="donut" height="300" :options="chartOptionsMedian" :series="medianSeries"/>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-md-4 mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-header">
                        <h5>Absolute Request Time</h5>
                    </div>
                    <div class="card-body">
                        <div id="chart">
                            <apexchart type="donut" height="300" :options="chartOptionsMedian" :series="absoluteSeries"/>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
import * as signalR from "@aspnet/signalr";
import axios from 'axios';
import ApexCharts from 'apexcharts';
export default {
    data:function () {
        return {
            sources: [],
            selSourceId:"",
            loading:false,
            requestModel:{
                "Debug": false,
                "Info": false,
                "Warn": true,
                "Error": true,
                "Critical": true,
                "FromTime": null,
                "ToTime": null,
                "From": 0,
                "Count": 50,
                "Normal": true,
                "Invoke": false,
                "Return": false,
                "Exec": true,
                "Rest": true,
                "IncludeRest": null,
                "ExcludeRest": null,
                "SourceId": null,
                "ThreadId": null,
            },
            countSeries:[],
            errorSeries:[],
            medianSeries:[],
            absoluteSeries:[],
            connection: null,
            currentResult:  {totalCount:0},
            selEndPoint:{endPoints:[]},
        }
    },
    async mounted(){
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/websocket")
            .build();
        await this.connection.start();
        this.getSources();
    },
    methods:{
        async getSources() {
            try {
                const response = await axios.get(`/api/Source/All`);
                for(var i = 0; i < response.data.length;i++){
                    response.data[i].FormattedName = response.data[i].Name + " - " + response.data[i].Tag + " / " + response.data[i].Version
                }
                this.sources = response.data;
                this.selSourceId = this.sources[0].SourceId;
            } catch (e) {
                console.log(e);
            }
        },
        async requestData(){
            this.loading = true;
            var req = JSON.parse(JSON.stringify(this.requestModel));
            req.SourceId = this.selSourceId;
            this.logs = [];
            var result = await this.connection.invoke("RequestAnalyseUsage",req);
            this.currentResult = result;
            this.selEndPoint = result;
            this.updateCharts();
            this.loading = false;
        },
        updateCharts(){
            this.countSeries = this.selEndPoint.endPoints.map(function(e){return e.requestCount});
            this.errorSeries = this.selEndPoint.endPoints.map(function(e){return e.errorCount});
            this.medianSeries = this.selEndPoint.endPoints.map(function(e){return e.medianRequestTime});
            this.absoluteSeries = this.selEndPoint.endPoints.map(function(e){return e.absoluteRequestTime});
        },
    },
    filters:{
        ticksToSecond(ticks){
            ticks = ticks * 1;
            if(ticks < 10000000)
                return (ticks/10000).toFixed(2) + " ms";
            else if(ticks < 600000000)
                return (ticks/10000000).toFixed(2) + " s";
            else if(ticks < 3600000000000)
                return (ticks/600000000).toFixed(2) + " min";
            else
                return (ticks/36000000000).toFixed(2) + " h";
        },
        max20LastLength(t){
            if(!t) return "";
            if(t.length > 20)
                return '...' + t.substr(t.length-20);
            return t;
        },
    },
    computed:{
        chartOptions: function() {
            var self = this;
            return {
                chart: {
                    events: {
                        dataPointSelection: function (event, chartContext, config) {
                            if(self.selEndPoint.endPoints[config.dataPointIndex].endPoints.length > 0){
                                self.selEndPoint = self.selEndPoint.endPoints[config.dataPointIndex];
                                self.updateCharts();
                            }
                        }
                    }
                },
                labels: this.selEndPoint.endPoints.map(function (p) { 
                    
                    if(p.endPoint.length > 25)
                        return '...' + p.endPoint.substr(p.endPoint.length-25);
                    else
                        return p.endPoint;
                }),
                legend:{
                    position:'bottom'
                }
            } 
        },
        chartOptionsMedian: function() {
            var self = this;
            return {
                chart: {
                    events: {
                        dataPointSelection: function (event, chartContext, config) {
                            console.log(config.dataPointIndex)
                            console.log(self.selEndPoint.endPoints[config.dataPointIndex].endPoints.length)
                            if(self.selEndPoint.endPoints[config.dataPointIndex].endPoints.length > 0){
                                self.selEndPoint = self.selEndPoint.endPoints[config.dataPointIndex];
                                self.updateCharts();
                            }
                        }
                    }
                },
                labels: this.selEndPoint.endPoints.map(function (p) { 

                    if(p.endPoint.length > 25)
                        return '...' + p.endPoint.substr(p.endPoint.length-25);
                    else
                        return p.endPoint;
                    }),
                legend:{
                    position:'bottom',
                },
                tooltip: {
                    y: {
                        formatter: function(value) {
                            value = value*1;
                            if(value < 10000000)
                                return value/10000 + " ms";
                            else if(value < 600000000)
                                return value/10000000 + " s";
                            else if(value < 3600000000000)
                                return value/600000000 + " min";
                            else
                                return value/36000000000 + " h";
                        }
                    }
                }
            } 
        },
        timeChartOptions: function(){
            return {
                chart: {
                    stacked: false,
                    zoom: {
                    type: 'x',
                    enabled: true,
                    autoScaleYaxis: true
                    },
                    toolbar: {
                    autoSelected: 'zoom'
                    }
                },
                plotOptions: {
                    line: {
                    curve: 'smooth',
                    }
                },
                dataLabels: {
                    enabled: false
                },

                markers: {
                    size: 0,
                    style: 'full',
                },
                //colors: ['#0165fc'],
                title: {
                    text: 'Requests over Time',
                    align: 'left'
                },
                fill: {
                    type: 'gradient',
                    gradient: {
                    shadeIntensity: 1,
                    inverseColors: false,
                    opacityFrom: 0.5,
                    opacityTo: 0,
                    stops: [0, 90, 100]
                    },
                },
                yaxis: {
                    labels: {
                    formatter: function (val) {
                        return (val / 1000000).toFixed(0);
                    },
                    },
                    title: {
                    text: 'Requests'
                    },
                },
                xaxis: {
                    type: 'datetime',
                },

                tooltip: {
                    shared: false,
                    y: {
                        formatter: function (val) {
                            return (val / 1000000).toFixed(0)
                        }
                    }
                }
            }
        }
    },
}
</script>