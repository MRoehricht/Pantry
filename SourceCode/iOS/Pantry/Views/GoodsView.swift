//
//  GoodsView.swift
//  Pantry
//
//  Created by Matthias Röhricht on 03.01.25.
//
import SwiftUI
import CodeScanner

struct GoodsView: View {
    @State private var isShowingScanner = false
    @State private var text = "Noch nix gescannt"
    
    var body: some View {
        
        NavigationStack {
            Text(text)
        .navigationTitle("Goods")
            .toolbar {
                ToolbarItem(placement: .topBarTrailing) {
                    Button("Scan", systemImage: "barcode.viewfinder") {
                        isShowingScanner = true
                    }
                }
            }
        
            .sheet(isPresented: $isShowingScanner) {
                CodeScannerView(codeTypes: [.ean13, .ean8], scanInterval:1.0, simulatedData: "4058172795664", completion: handleScan)
                
            }
        }
    }
    func handleScan(result: Result<ScanResult, ScanError>) {
            isShowingScanner = false

            switch result {
            case .success(let result):
                text = result.string
                print("Found code: \(result.string)")
               
            case .failure(let error):
                text = "Scanning failed: \(error.localizedDescription)"
                print("Scanning failed: \(error.localizedDescription)")
            }
        }
}

#Preview {
    GoodsView()
}
