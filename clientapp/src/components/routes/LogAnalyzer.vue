<template>
    <div class="container-fluid">
        <div class="d-sm-flex align-items-center justify-content-between mb-4">
            <h2 class="h3 mb-0 text-gray-800">Log Analyzer</h2>
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
                                <b-dropdown text="Log Level" variant="primary" class="m-2" tabindex="-1">
                                    <b-dropdown-form>
                                        <b-form-checkbox v-model="requestModel.Debug" @change="stopLoop">
                                            <font-awesome-icon icon="bug" /> Debug
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Info" @change="stopLoop">
                                            <font-awesome-icon icon="info" /> Information
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Warn" @change="stopLoop">
                                            <font-awesome-icon icon="exclamation-circle" /> Warning
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Error" @change="stopLoop">
                                            <font-awesome-icon icon="times" /> Error
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Critical" @change="stopLoop">
                                            <font-awesome-icon icon="skull-crossbones" /> Critical
                                        </b-form-checkbox>
                                    </b-dropdown-form>
                                </b-dropdown>
                                <b-dropdown text="Log Typ" variant="info" class="m-2" tabindex="-1">
                                    <b-dropdown-form>
                                        <b-form-checkbox v-model="requestModel.Normal" @change="stopLoop">
                                            <font-awesome-icon icon="info" /> Normal
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Invoke" @change="stopLoop">
                                            <font-awesome-icon icon="play" /> Invoke
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Return" @change="stopLoop">
                                            <font-awesome-icon icon="undo" /> Return
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Exec" @change="stopLoop">
                                            <font-awesome-icon icon="exclamation" /> Exec
                                        </b-form-checkbox>
                                        <b-form-checkbox v-model="requestModel.Rest" @change="stopLoop">
                                            <font-awesome-icon icon="bullhorn" /> Rest
                                        </b-form-checkbox>
                                    </b-dropdown-form>
                                </b-dropdown>
                                <datetime input-class="form-control m-2" v-model="requestModel.FromTime" title="From" type="datetime" format="dd.MM.yyyy hh:mm:ss" @change="stopLoop"></datetime>
                                <datetime input-class="form-control m-2" v-model="requestModel.ToTime" title="To" type="datetime" format="dd.MM.yyyy hh:mm:ss" @change="stopLoop"></datetime>
                                <label class="mr-sm-2">Include Path</label>
                                <b-input
                                    class="mb-2 mr-sm-2 mb-sm-0"
                                    v-model="requestModel.IncludePath"
                                    placeholder="Include Path"
                                     @change="stopLoop"
                                    ></b-input>
                                <label class="mr-sm-2">Exclude Path</label>
                                <b-input
                                    class="mb-2 mr-sm-2 mb-sm-0"
                                    v-model="requestModel.ExcludePath"
                                    placeholder="Exclude Path"
                                     @change="stopLoop"
                                    ></b-input>
                                <b-button @click="requestData()" variant="success" class="float-right"><b-spinner v-if="loading" label="Spinning" small></b-spinner> Request</b-button>
                            </b-form>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="row">
            <div class="col mb-4">
                <div class="card border-left-primary shadow h-100 py-2">
                    <div class="card-body">
                        <div class="d-sm-flex align-items-center justify-content-between mb-4">
                            <h5 class="h3 mb-0 text-gray-800">Results - {{logs.length}} / {{currentResult.totalCount}}</h5> 
                        </div>
                        <div>
                            <b-table
                                small
                                striped hover
                                sticky-header="550px"
                                :items="logs"
                                :fields="fields"
                                @row-clicked="togD"
                                :tbody-tr-class="rowClass">

                                <template v-slot:row-details="row">
                                    <ul>
                                        <li v-for="(value, key) in row.item" :key="key">{{ key }}: {{ value }}</li>
                                    </ul>
                                </template>
                                <template v-slot:cell(time)="row">
                                    <span>{{row.value | timeDate}}</span>
                                    <br/>
                                    <span>{{row.value | timeTime}}</span>
                                </template>
                                <template v-slot:cell(logTyp)="row">
                                    <font-awesome-icon :icon="logTypIcon(row.item.logTyp)" /> 
                                </template>
                                <template v-slot:cell(logLevel)="row">
                                    <font-awesome-icon :icon="logLevelIcon(row.item.logLevel)" /> 
                                </template>

                                <template v-slot:cell(class)="row">
                                    {{row.value | max20LastLength}}.{{row.item.method}}:{{row.item.line}}
                                </template>


                                <template v-slot:cell(message)="row">
                                    {{row.item | messageException}}
                                </template>

                            </b-table>
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
export default {
    components:{
        signalR,
    },
    data:function () {
        return {
            sources: [],
            selSourceId:"",
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
                "ThreadId": null
            },
            loop:false,
            loading: false,
            connection: null,
            logs:[],
            currentResult:  {totalCount:0},
            fields:[
                { key: 'id', label: 'ID', sortable: true, sortDirection: 'desc' },
                { key: 'time', label: 'Time', sortable: true, sortDirection: 'desc' },
                { key: 'logTyp', label: 'Typ', sortable: true, sortDirection: 'desc' },
                { key: 'logLevel', label: 'Level', sortable: true, sortDirection: 'desc' },
                { key: 'class', label: 'Path', sortable: true, sortDirection: 'desc' },
                { key: 'message', label: 'Message', sortable: true, sortDirection: 'desc' },
            ]
        }
    },
    async mounted(){
        this.connection = new signalR.HubConnectionBuilder()
            .withUrl("/websocket")
            .build();
        await this.connection.start();
        this.getData();
    },
    methods:{
        async requestData(){
            if(this.loop) {
                this.loop = false;
                return;
            }
            this.loop = true;
            this.loading = true;
            var req = JSON.parse(JSON.stringify(this.requestModel));
            req.SourceId = this.selSourceId;
            this.logs = [];
            do{
                var result = await this.connection.invoke("Request",req);
                this.logs = this.logs.concat(result.logs);
                this.currentResult = result;
                req.From += req.Count;
            }while(this.loop && this.currentResult.totalCount > this.logs.length)
            this.loading = false;
            this.loop = false;
        },
        sleep(ms) {
            return new Promise(resolve => setTimeout(resolve, ms));
        },
        async getData() {
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
        togD(row) {
            row._showDetails = !row._showDetails;
            this.$forceUpdate();
        },
        stopLoop(d){
            this.loop = false;
        },
        rowClass(item, type) {
            if (!item) return
            if (item.logLevel === 3) return 'table-warning';
            if (item.logLevel === 4) return 'table-danger';
            if (item.logLevel === 5) return 'table-danger';
            if (item.logTyp === 1) return 'table-danger';
        },
        logTypIcon(typ){
            if(typ == 0) return 'info';
            if(typ == 1) return 'exclamation';
            if(typ == 2) return 'undo';
            if(typ == 3) return 'play';
            if(typ == 4) return 'bullhorn';
        },
        logLevelIcon(lvl){
            if(lvl == 0) return 'bug';
            if(lvl == 1) return 'bug';
            if(lvl == 2) return 'info';
            if(lvl == 3) return 'exclamation-circle';
            if(lvl == 4) return 'times';
            if(lvl == 5) return 'skull-crossbones';
        },
    },
    computed: {
    },
    filters:{
        timeDate(t){
            return t.split('T')[0];
        },
        timeTime(t){
            return t.split('T')[1];
        },
        max20LastLength(t){
            if(t.length > 20)
                return '...' + t.substr(t.length-20);
            return t;
        },
        max20Length(t){
            if(t.length > 20)
                return t.substring(0,20) + '...';
            return t;
        },
        messageException(item){
            var result = "";
            if(item.message && item.message != "") result = item.message;
            else if(item.exception && item.exception != "") result = item.exception;
            else if(item.data && item.data != "") result = item.data;
            if(result.length > 200)
                return result.substring(0,200) + '...';
            return result;
        }
    }
}
</script>