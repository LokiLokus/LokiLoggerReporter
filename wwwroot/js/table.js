var app = new Vue({
    data:function(){
        return {
            data: [],
            show: true,
            totalRows: 1,
            currentPage: 1,
            perPage: 30,
            source: '',
            count: 100,
            pageOptions: [10, 30, 50, 100],
            sortBy: null,
            sortDesc: false,
            sortDirection: 'asc',
            from: new Date(),
            to: new Date(),
            fields: [
                {key: 'LogLevel', label: 'Lvl', sortable: true, class: 'text-center'},
                {key: 'LogTyp', label: 'Typ', sortable: true, class: 'text-center'},
                {key: 'Time', label: 'Time', sortable: true},
                {key: 'Message', label: 'Nachricht', sortable: true}
            ],
            filter: null,
            debug: true,
            info: true,
            warn: true,
            error: true,
            crit: true,
            norm: true,
            except: true,
            retur: true,
            invoke: true,
            restcall:true,
            classFilter: '',
            after: '',
            sources: [],
            selSource: 0
        }
    },
    methods: {

        rowClass(item, type) {
            if (!item) return
            if (item.LogLevel === 3) return 'warnRow';
            if (item.LogLevel === 4) return 'errRow';
            if (item.LogLevel === 5) return 'critRow';
        },
        getAllSources:function(){
            axios.get('/api/Source/All')
                .then(x => {
                    this.sources = x.data;
                    if(x.data.length > 0){
                        this.selSource = 0;
                        this.getData();
                    }
                }).catch(x => {
                if(x.response){
                    this.errors = x.response.data;
                }else{
                    console.log(x);
                    alert("Ein Fehler ist aufgetreten");
                }
            });
        },
        getSource: function(){
            var url = window.location.pathname;
            var urls = url.split("/");
            var path;
            for (var i = 0; i < urls.length; i++) {
                if(urls[i] === "Table"){
                    if(urls.length > i){
                        this.source = urls[i+1];
                        break;
                    }
                }
            }
        },
        getData: function () {
            if(this.sources[this.selSource]){
                axios.get('/api/Logging/GetLogBySource/' + this.sources[this.selSource].SourceId + '/0-' + this.count)
                    .then(x => {
                        this.data = x.data;
                    }).catch(x => {
                    if(x.response){
                        this.errors = x.response.data;
                    }else{
                        console.log(x);
                        alert("Ein Fehler ist aufgetreten");
                    }
                });
            }
        },
        expandAdditionalInfo: function(row){
            row._showDetails = !row._showDetails;
            this.$forceUpdate();
        },
      onFiltered(filteredItems) {
        this.totalRows = filteredItems.length;
        this.currentPage = 1
      }
    },
    computed: {
        sortOptions() {
            return this.fields
                .filter(f => f.sortable)
                .map(f => {
                    return { text: f.label, value: f.key }
                })
        },
        filterd: function () {
            var result = this.data;// moment(this.after,"YYYY-MM-DD HH:mm:ss");
            /*
            result = result.filter(x => {
                
                if(x.Time >= this.after) return true;
                return false;
            });
            */
            result = result.filter(xa =>{
                if((xa.LogLevel === 0 || xa.LogLevel === 1)&& this.debug) return true;
                if(xa.LogLevel === 2 && this.info) return true;
                if(xa.LogLevel === 3 && this.warn) return true;
                if(xa.LogLevel === 4 && this.error) return true;
                if(xa.LogLevel === 5 && this.crit) return true;
                return false;
            });
            
            result = result.filter(xa => {
                if(xa.LogTyp === 0 && this.norm) return true;
                if(xa.LogTyp === 1 && this.except) return true;
                if(xa.LogTyp === 2 && this.retur) return true;
                if(xa.LogTyp === 3 && this.invoke) return true;
                if(xa.LogTyp === 4 && this.restcall) return true;
                return false;
            });
            if(this.classFilter !== ''){
                result = result.filter (xa =>{
                    if(xa.Class && xa.Class.toLowerCase().indexOf(this.classFilter.toLowerCase()) !== -1) return true;
                    if(xa.Method && xa.Method.toLowerCase().indexOf(this.classFilter.toLowerCase()) !== -1) return true;
                    return false;
                });
            }
            
            this.totalRows = result.length;
            return result;
        }
    },
    mounted() {
        this.getAllSources();
        this.after = moment(new Date(),"YYYY-MM-DD HH:mm:ss");
        this.getSource();
        this.getData();
    },
    filters: {
        level:function(lvl){
            if(lvl == 0 || lvl == 1) return "Debug";
            if(lvl == 2) return "Information";
            if(lvl == 3) return "Warning";
            if(lvl == 4) return "Error";
            if(lvl == 5) return "Critical";
            return "";
        },
        typ:function(typ){
            if(typ == 0) return "";
            if(typ == 1) return "Exception";
            if(typ == 2) return "Return";
            if(typ == 3) return "Invoke";
            if(typ == 4) return "RestCall";
            return "";
        },
        nBreak:function(br){
            if(br == null) return "";
            return br.replace(/(\\n)/g, '<br>');
        },
        date: function (value) {
            if (!value) return '';
            var t = new Date(value);
            var dd = t.getDate();
            var mm = t.getMonth()+1;
            var yyyy = t.getFullYear();
            var hh = t.getHours();
            var mMin = t.getMinutes();
            var ss = t.getSeconds();
            
            if(dd<10){
                dd='0'+dd
            }
            if(mm<10){
                mm='0'+mm
            }
            return yyyy + '.' + mm + '.' + dd + ' - ' + hh + ':' + mMin + ':' + ss ;
        },
        message:function (value) {
            if (!value || !value.Message) return '';
            var datasets = [];
            var re = / {[\s\S]*?}/;
            var message = value.Message;
            var result = "";
            
            try{
                datasets = JSON.parse(value.Data);
            }catch(err){}
            
            if(Array.isArray(datasets)){
                for (let i = 0; i < datasets.length; i++) {
                    match = re.exec(message);
                    if(match != null){
                        var tmp = message.substr(match.index + match[0].length);
                        result += message.substr(0, match.index) + ' ' + JSON.stringify(datasets[i]);
                        
                        message = tmp;
                    }
                }
                
            }
            value.Message = result;
            
            return result.substr(0,100) + "...";
        },

        
        renderDetail:function (item) {
            //Don|t ask me why this fucking shit can't be used in a normal Method
            shortStr = function (str) {
                if(!str) str = '';
                //if(str.length > 100){
                //    return str.substring(0,100) + '...';
                //}
                return str;
            };
            getTr = function (key,val) {
                return "<tr><td>" + key + "</td><td>" + val + "</td></tr>"; 
            };
            
            var result = "";
            
            if(item.LogTyp === 4){
                var data = JSON.parse(item.Data)[0];
                result += "<table>";
                result += "<tbody>";
                result += getTr("Location",data.Class + '.' + data.method + ':' + data.Line);
                result += getTr("Method",data.HttpMethod);
                result += getTr("Path",data.Scheme + "//" + data.Host + data.Path + data.QueryString);
                result += getTr("Status Code",data.StatusCode);
                result += getTr("Request",shortStr(data.RequestBody));
                result += getTr("Exception",shortStr(data.Exception));
                result += getTr("Response",shortStr(data.ResponseBody.replace(/</g,'&lt;').replace(/>/g,'&gt;')));
                result += getTr("Client Ip",data.ClientIp);
                result += getTr("Trace Id",data.TraceId);
                result += getTr("Start",data.Start);
                result += getTr("End",data.End);
                result += getTr("Executen Time",(new Date(data.End)).getTime() -  (new Date(data.Start)).getTime() + " Milli Seconds");
                result += "</tbody></table>"
                
            }
            return result;
        }
    },
    el:"#tableAnalyze"
});