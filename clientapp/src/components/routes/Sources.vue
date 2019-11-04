<template>
  <div class="container-fluid">
    <!-- Page Heading -->
    <div class="d-sm-flex align-items-center justify-content-between mb-4">
      <h1 class="h3 mb-0 text-gray-800">Sources</h1>
      <button class="d-none d-sm-inline-block btn btn-sm btn-primary shadow-sm" @click="newSource()">
        <i class="fas fa-download fa-sm text-white-50"></i> Add Sources
      </button>
    </div>

    <div class="row">
      <div class="col-md-4 mb-4">
        <div class="card border-left-primary shadow h-100 py-2">
          <div class="card-body">
            <div class="row no-gutters align-items-center">
              <div class="col mr-2">
                <div
                  class="text font-weight-bold text-primary text-uppercase mb-1">Sources</div>
              </div>
              <div class="col-auto">
                  <b-table striped hover
                  small
                  :items="sources"
                  :fields="fields"
                  @row-clicked="select"></b-table>
              </div>
            </div>
            </div>
          </div>
        </div>
        <source-detail v-on:reload="getData()" v-if="selSource !== null" :source="selSource"></source-detail>
    </div>
  </div>
</template>

<script lang="ts">
import axios from 'axios';
import Vue from 'vue'
import SourceDetail from './detail/SourceDetail.vue';
export default Vue.extend({
    components:{
        SourceDetail,
    },
    data:function () {
        return {
            sources: [],
            fields:["Name","Version","Tag"],
            selSource:null
        }
    },
    mounted(){
        this.getData();
    },
    methods:{
        select(i:any){
            console.log("source")
            this.selSource = i;
        },
        async getData() {
            try {
                const response = await axios.get(`/api/Source/All`);
                this.sources = response.data;
            } catch (e) {
                console.log(e);
            }
        },
        newSource(){
            this.selSource =  {
                Name:'',
                Tag:'',
                Version:'',
                Secret:''
            };
        }
    }
})
</script>

